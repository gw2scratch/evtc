using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GW2Scratch.ArcdpsLogManager.Logs;
using Newtonsoft.Json;

namespace GW2Scratch.ArcdpsLogManager.Uploads
{
	public class DpsReportUploader : IDisposable
	{
		public static readonly DpsReportDomain DefaultDomain = new DpsReportDomain("https://dps.report",
			"Cloudflare. Supports HTTPS. May be unreliable in Eastern European and Asian countries");

		private static readonly DpsReportDomain DomainA = new DpsReportDomain("http://a.dps.report",
			"Imperva. Supports HTTP ONLY. Fairly reliable, use as last resort.");

		private static readonly DpsReportDomain DomainB = new DpsReportDomain("https://b.dps.report",
			"Stackpath. Supports HTTPS. Alternative service domain that supports HTTPS.");

		public static IReadOnlyList<DpsReportDomain> AvailableDomains { get; } = new[]
		{
			DefaultDomain, DomainA, DomainB
		};

		private const string UploadEndpoint = "/uploadContent";

		private readonly HttpClient httpClient = new HttpClient
		{
			Timeout = Timeout.InfiniteTimeSpan
		};

		public string Domain { get; set; }

		public DpsReportUploader() : this(DefaultDomain)
		{
		}

		public DpsReportUploader(DpsReportDomain domain)
		{
			Domain = domain.Domain;
		}

		public DpsReportUploader(string domain)
		{
			Domain = domain;
		}

		public async Task<DpsReportResponse> UploadLogAsync(LogData log, CancellationToken cancellationToken,
			string userToken = null)
		{
			string query = Domain + UploadEndpoint + "?json=1&generator=ei";
			if (userToken != null)
			{
				query += $"&userToken={userToken}";
			}

			bool isZipped = IsZippedLog(log.FileName);

			HttpResponseMessage response;
			using (var content = new MultipartFormDataContent())
			{
				if (isZipped)
				{
					await using (var stream = log.FileInfo.OpenRead())
					{
						content.Add(new StreamContent(stream), "file", log.FileInfo.Name);
						response = await httpClient.PostAsync(query, content, cancellationToken);
					}
				}
				else
				{
					string filename = $"{log.FileInfo.Name}.zevtc";

					await using var evtcStream = log.FileInfo.OpenRead();

					await using var archiveMemoryStream = new MemoryStream();
					using (var archive = new ZipArchive(archiveMemoryStream, ZipArchiveMode.Create, true))
					{
						var entry = archive.CreateEntry(filename);

						await using var entryStream = entry.Open();
						evtcStream.CopyTo(entryStream);
					}

					archiveMemoryStream.Seek(0, SeekOrigin.Begin);
					content.Add(new StreamContent(archiveMemoryStream), "file", filename);
					response = await httpClient.PostAsync(query, content, cancellationToken);
				}
			}

			string json = await response.Content.ReadAsStringAsync();
			var responseData = JsonConvert.DeserializeObject<DpsReportResponse>(json);

			return responseData;
		}

		public Task<DpsReportResponse> UploadLogAsync(LogData log, string userToken = null)
		{
			return UploadLogAsync(log, CancellationToken.None, userToken);
		}

		public void Dispose()
		{
			httpClient?.Dispose();
		}

		private bool IsZippedLog(string filename)
		{
			// This is not exactly the best approach, but as far as I know there is no built-in
			// function for checking if a zip is valid.
			try
			{
				using var archive = ZipFile.OpenRead(filename);
				return archive.Entries.Count > 0;
			}
			catch
			{
				return false;
			}
		}
	}
}