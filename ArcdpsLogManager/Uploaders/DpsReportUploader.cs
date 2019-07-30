using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GW2Scratch.ArcdpsLogManager.Logs;
using Newtonsoft.Json;

namespace GW2Scratch.ArcdpsLogManager.Uploaders
{
	public class DpsReportUploader : IDisposable
	{
		private const string UploadEndpoint = "https://dps.report/uploadContent";

		private readonly HttpClient httpClient = new HttpClient();

		public async Task<DpsReportResponse> UploadLogAsync(LogData log, CancellationToken cancellationToken,
			string userToken = null)
		{
			string query = UploadEndpoint + "?json=1&generator=ei";
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