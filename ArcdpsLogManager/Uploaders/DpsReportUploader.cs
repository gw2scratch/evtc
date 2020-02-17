using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GW2Scratch.ArcdpsLogManager.Logs;
using Newtonsoft.Json;

namespace GW2Scratch.ArcdpsLogManager.Uploaders
{
	public class DpsReportUploader : IDisposable
	{
		public static readonly DpsReportDomain DefaultDomain = new DpsReportDomain("https://dps.report",
			"Cloudflare. Supports HTTPS. May be unreliable in Eastern European and Asian countries");

		private static readonly DpsReportDomain DomainA = new DpsReportDomain("http://a.dps.report",
			"Imperva. Supports HTTP ONLY. Fairly reliable, use as last resort.");

		private static readonly DpsReportDomain DomainB = new DpsReportDomain("https://b.dps.report",
			"Stackpath. Supports HTTPS. New service domain, still testing.");

		public static IReadOnlyList<DpsReportDomain> AvailableDomains { get; } = new[]
		{
			DefaultDomain, DomainA, DomainB
		};

		private const string UploadEndpoint = "/uploadContent";

		private readonly HttpClient httpClient = new HttpClient();

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

			HttpResponseMessage response;
			using (var content = new MultipartFormDataContent())
			using (var stream = log.FileInfo.OpenRead())
			{
				content.Add(new StreamContent(stream), "file", log.FileInfo.Name);
				response = await httpClient.PostAsync(query, content, cancellationToken);
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
	}
}