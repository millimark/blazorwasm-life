using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace BlazorWasmLife.Shared
{
    /// <summary>
    /// A service class to interface with the ConwayWebAPI 
    /// </summary>
    public class ConwayService
    {
        private readonly HttpClient _httpClient;

        public ConwayService(HttpClient client)
        {
            _httpClient = client;
        }

        /// <summary>
        /// Gets a lifeboard matrix 
        /// </summary>
        /// <param name="pattern">Name of pattern</param>
        /// <returns>a LifeBoardInt object that contains as its initial pattern the
        /// pattern named by pattern
        /// </returns>
        public async Task<LifeBoardInt> GetPatternAsync(string pattern)
        {
            var response = await _httpClient.GetAsync(new Uri($"conwaylife/{pattern}", UriKind.Relative)).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            using (var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                return await JsonSerializer.DeserializeAsync
                    <LifeBoardInt>(responseStream);
            }
        }

        /// <summary>
        /// Applies rules to a pattern and returns the next generation
        /// </summary>
        /// <param name="cells">current </param>
        /// <returns>the next generation pattern after applying Life rules</returns>
        public async Task<LifeBoardInt> GetNextGeneration(LifeBoardInt cells)
        {
            using (MemoryStream ms = new MemoryStream())
            {

                await JsonSerializer.SerializeAsync<LifeBoardInt>(ms, cells).ConfigureAwait(false);
                ms.Position = 0;
                using (StreamReader sr = new StreamReader(ms))
                {
                    var str = sr.ReadToEnd();

                    using (StringContent body = new StringContent(str, Encoding.UTF8, "application/json"))
                    {
                        var response = await _httpClient.PostAsync(new Uri("conwaylife", UriKind.Relative), body).ConfigureAwait(false);

                        response.EnsureSuccessStatusCode();

                        using (var responseStream
                                = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                        {
                            return await JsonSerializer.DeserializeAsync
                                <LifeBoardInt>(responseStream).ConfigureAwait(false);
                        }
                    }
                }
            }
        }
    }
}
