using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ArcdpsLogManager.Logs;
using Newtonsoft.Json;

namespace ArcdpsLogManager.Uploaders
{
	public class DpsReportUploader : IUploader
	{
		private class DpsReportResponse
		{
            public string Id { get; set; }
            public string Permalink { get; set; }
            public string UserToken { get; set; }
		}

		private const string UploadEndpoint = "https://dps.report/uploadContent";


		private readonly HttpClient httpClient = new HttpClient();

		public async Task<string> UploadLogAsync(LogData log, CancellationToken cancellationToken)
		{
            const string query = UploadEndpoint + "?json=1&generator=ei";

            HttpResponseMessage response;
            using (var content = new MultipartFormDataContent())
            using (var stream = log.FileInfo.OpenRead())
            {
	            content.Add(new StreamContent(stream), "file", log.FileInfo.Name);
	            response = await httpClient.PostAsync(query, content, cancellationToken);
            }

            string json = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<DpsReportResponse>(json);

            return responseData.Permalink;
		}

		public Task<string> UploadLogAsync(LogData log)
		{
			return UploadLogAsync(log, CancellationToken.None);
		}
	}
}