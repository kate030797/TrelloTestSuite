using RestSharp;

namespace TrelloTestSuite
{
    public class TrelloApiClient
    {
        private readonly string _apiKey;
        private readonly string _apiToken;
        private readonly RestClient _client;

        public TrelloApiClient(string apiKey, string apiToken)
        {
            _apiKey = apiKey;
            _apiToken = apiToken;
            _client = new RestClient("https://api.trello.com/1/");
        }

        public async Task<RestResponse> CreateBoard(string boardName)
        {
            var request = new RestRequest("boards", Method.Post);

            SetAuthorizationParameters(request);
            request.AddQueryParameter("name", boardName);

            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse> GetBoard(string boardId)
        {
            var request = new RestRequest($"boards/{boardId}", Method.Get);

            SetAuthorizationParameters(request);

            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse> UpdateBoard(string boardId, string boardName)
        {
            var request = new RestRequest($"boards/{boardId}", Method.Put);

            SetAuthorizationParameters(request);
            request.AddQueryParameter("name", boardName);

            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse> DeleteBoard(string boardId)
        {
            var request = new RestRequest($"boards/{boardId}", Method.Delete);

            SetAuthorizationParameters(request);

            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse> CreateList(string listName, string boardId)
        {
            var request = new RestRequest("lists", Method.Post);

            SetAuthorizationParameters(request);
            request.AddQueryParameter("name", listName);
            request.AddQueryParameter("idBoard", boardId);

            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse> GetList(string listId)
        {
            var request = new RestRequest($"lists/{listId}", Method.Get);

            SetAuthorizationParameters(request);

            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse> UpdateList(string listId, string listName)
        {
            var request = new RestRequest($"lists/{listId}", Method.Put);

            SetAuthorizationParameters(request);
            request.AddQueryParameter("name", listName);

            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse> CreateCard(string cardName, string cardDesc, string listId)
        {
            var request = new RestRequest($"cards", Method.Post);

            SetAuthorizationParameters(request);
            request.AddQueryParameter("name", cardName);
            request.AddQueryParameter("desc", cardDesc);
            request.AddQueryParameter("idList", listId);

            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse> GetCard(string cardId)
        {
            var request = new RestRequest($"cards/{cardId}", Method.Get);

            SetAuthorizationParameters(request);

            return await _client.ExecuteAsync(request);
        }

        public async Task<RestResponse> UpdateCard(string cardId, string cardName, string cardDesc)
        {
            var request = new RestRequest($"cards/{cardId}", Method.Put);

            SetAuthorizationParameters(request);
            request.AddQueryParameter("name", cardName);
            request.AddQueryParameter("desc", cardDesc);

            return await _client.ExecuteAsync(request);
        }

        private void SetAuthorizationParameters(RestRequest request)
        {
            request.AddQueryParameter("key", _apiKey);
            request.AddQueryParameter("token", _apiToken);
        }
    }
}