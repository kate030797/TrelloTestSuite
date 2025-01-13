using Newtonsoft.Json.Linq;
using System.Net;

namespace TrelloTestSuite
{
    [TestFixture]
    public class TrelloTests
    {
        private TrelloApiClient _client;
        private TrelloApiClient _nonAuthorizedClient;
        private string _boardId = "";
        private string _listId = "";
        private string _cardId = "";

        [SetUp]
        public void Setup()
        {
            // Inicjalizacja klienta API z kluczem API i tokenem autoryzacyjnym
            string apiKey = "";
            string apiToken = "";
            _client = new TrelloApiClient(apiKey, apiToken);

            // Inicjalizacja klienta API bez autoryzacji (do testów nieautoryzowanych)
            _nonAuthorizedClient = new TrelloApiClient("", "");
        }

        [Test, Order(1)]
        public async Task CreateBoard_ShouldReturnSuccess()
        {
            // Test tworzenia nowej planszy
            var response = await _client.CreateBoard("New board");

            // Sprawdzamy, czy odpowiedź jest poprawna i nie pusta
            Assert.Multiple(() =>
            {
                Assert.That(response.IsSuccessful, Is.True, "Board creation failed.");
                Assert.That(response.Content, Is.Not.Null.And.Not.Empty, "Board creation failed. There is no response.");
            });

            // Parsowanie odpowiedzi JSON i zapisywanie ID planszy
            var responseContent = JObject.Parse(response.Content);
            _boardId = $"{responseContent["id"]}";

            // Sprawdzamy, czy ID planszy jest niepuste
            Assert.That(_boardId, Is.Not.Null.And.Not.Empty, "Board ID is null or empty.");
        }

        [Test, Order(2)]
        public async Task CreateBoard_ShouldReturnBadRequest()
        {
            // Test próby stworzenia planszy bez podania nazwy
            var response = await _client.CreateBoard("");

            // Sprawdzamy, czy odpowiedź zawiera kod błędu 400 (BadRequest)
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "Board with no name was created.");
        }

        [Test, Order(3)]
        public async Task GetBoard_ShouldReturnSuccess()
        {
            // Test pobierania planszy po jej utworzeniu
            var response = await _client.GetBoard(_boardId);

            // Sprawdzamy, czy odpowiedź jest poprawna i nie pusta
            Assert.Multiple(() =>
            {
                Assert.That(response.IsSuccessful, Is.True, "Board retrive failed.");
                Assert.That(response.Content, Is.Not.Null.And.Not.Empty, "There is no response.");
            });

            // Parsowanie odpowiedzi JSON i porównanie ID planszy
            var responseContent = JObject.Parse(response.Content);
            string boardId = $"{responseContent["id"]}";
            Assert.That(boardId, Is.EqualTo(_boardId), "Board retrive failed. Ids does not match.");
        }

        [Test, Order(4)]
        public async Task GetBoard_ShouldReturnNotFound()
        {
            // Test próby pobrania planszy o nieistniejącym ID
            string fakeId = BitConverter.ToString(Guid.NewGuid().ToByteArray(), 0, 12).Replace("-", "").ToLower();
            var response = await _client.GetBoard(fakeId);

            // Sprawdzamy, czy odpowiedź zawiera kod błędu 404 (NotFound)
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "Board with non existen ID was returned.");
        }

        [Test, Order(5)]
        public async Task UpdateBoard_ShouldReturnSuccess()
        {
            // Test aktualizacji nazwy planszy
            string newName = "Updated board";
            var response = await _client.UpdateBoard(_boardId, newName);

            // Sprawdzamy, czy odpowiedź jest poprawna
            Assert.That(response.IsSuccessful, Is.True, "Board update failed.");

            // Pobieramy zaktualizowaną planszę i sprawdzamy poprawność
            response = await _client.GetBoard(_boardId);
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty, "There is no response.");
            var responseContent = JObject.Parse(response.Content);
            string boardId = $"{responseContent["id"]}";
            string boardName = $"{responseContent["name"]}";

            // Sprawdzamy, czy ID planszy się zgadza i nazwa została zaktualizowana
            Assert.Multiple(() =>
            {
                Assert.That(boardId, Is.EqualTo(_boardId), "Board update failed. Ids does not match.");
                Assert.That(newName, Is.EqualTo(boardName), "Board update failed. Name does not match.");
            });
        }

        [Test, Order(6)]
        public async Task CreateBoard_ShouldReturnUnauthorized()
        {
            // Test próby utworzenia planszy bez autoryzacji
            var response = await _nonAuthorizedClient.CreateBoard("New board");

            // Sprawdzamy, czy odpowiedź zawiera kod błędu 401 (Unauthorized)
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized), "Board created but unauthorized.");
        }

        [Test, Order(7)]
        public async Task CreateList_ShouldReturnSuccess()
        {
            // Test tworzenia listy na istniejącej planszy
            var response = await _client.CreateList("New list", _boardId);

            // Sprawdzamy, czy odpowiedź jest poprawna i nie pusta
            Assert.Multiple(() =>
            {
                Assert.That(response.IsSuccessful, Is.True, "List creation failed.");
                Assert.That(response.Content, Is.Not.Null.And.Not.Empty, "List creation failed. There is no response.");
            });

            // Parsowanie odpowiedzi JSON i zapisywanie ID listy
            var responseContent = JObject.Parse(response.Content);
            _listId = $"{responseContent["id"]}";

            // Sprawdzamy, czy ID listy jest niepuste
            Assert.That(_listId, Is.Not.Null.And.Not.Empty, "List ID is null or empty.");
        }

        [Test, Order(8)]
        public async Task CreateList_ShouldReturnBadRequest()
        {
            // Test próby stworzenia listy bez nazwy
            var response = await _client.CreateList("", _boardId);

            // Sprawdzamy, czy odpowiedź zawiera kod błędu 400 (BadRequest)
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "List with no name was created.");
        }

        [Test, Order(9)]
        public async Task GetList_ShouldReturnSuccess()
        {
            // Test pobierania listy po jej utworzeniu
            var response = await _client.GetList(_listId);

            // Sprawdzamy, czy odpowiedź jest poprawna i nie pusta
            Assert.Multiple(() =>
            {
                Assert.That(response.IsSuccessful, Is.True, "Get list failed.");
                Assert.That(response.Content, Is.Not.Null.And.Not.Empty, "Get list failed. There is no response.");
            });

            // Parsowanie odpowiedzi JSON i porównanie ID listy
            var responseContent = JObject.Parse(response.Content);
            string listId = $"{responseContent["id"]}";
            Assert.That(listId, Is.EqualTo(_listId), "List Id does not match.");
        }

        [Test, Order(10)]
        public async Task GetList_ShouldReturnNotFound()
        {
            // Test próby pobrania listy o nieistniejącym ID
            string fakeId = BitConverter.ToString(Guid.NewGuid().ToByteArray(), 0, 12).Replace("-", "").ToLower();
            var response = await _client.GetList(fakeId);

            // Sprawdzamy, czy odpowiedź zawiera kod błędu 404 (NotFound)
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "List with non existing ID was returned.");
        }

        [Test, Order(11)]
        public async Task UpdateList_ShouldReturnSuccess()
        {
            // Test aktualizacji nazwy listy
            string newName = "Updated list";
            var response = await _client.UpdateList(_listId, newName);

            // Sprawdzamy, czy odpowiedź jest poprawna
            Assert.That(response.IsSuccessful, Is.True, "List update failed.");

            // Pobieramy zaktualizowaną listę i sprawdzamy poprawność
            response = await _client.GetList(_listId);
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty, "There is no response.");
            var responseContent = JObject.Parse(response.Content);
            string listId = $"{responseContent["id"]}";
            string listName = $"{responseContent["name"]}";

            // Sprawdzamy, czy ID listy się zgadza i nazwa została zaktualizowana
            Assert.Multiple(() =>
            {
                Assert.That(listId, Is.EqualTo(_listId), "List update failed. Ids does not match.");
                Assert.That(newName, Is.EqualTo(listName), "List update failed. Name does not match.");
            });
        }

        [Test, Order(12)]
        public async Task CreateList_ShouldReturnUnauthorized()
        {
            // Test próby utworzenia listy bez autoryzacji
            var response = await _nonAuthorizedClient.CreateList("New list", _boardId);

            // Sprawdzamy, czy odpowiedź zawiera kod błędu 401 (Unauthorized)
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized), "List created but unauthorized.");
        }

        [Test, Order(13)]
        public async Task CreateCard_ShouldReturnSuccess()
        {
            // Test tworzenia karty na istniejącej liście
            var response = await _client.CreateCard("New card", "New description", _listId);

            // Sprawdzamy, czy odpowiedź jest poprawna i nie pusta
            Assert.Multiple(() =>
            {
                Assert.That(response.IsSuccessful, Is.True, "Card creation failed.");
                Assert.That(response.Content, Is.Not.Null.And.Not.Empty, "Card creation failed. There is no response.");
            });

            // Parsowanie odpowiedzi JSON i zapisywanie ID karty
            var responseContent = JObject.Parse(response.Content);
            _cardId = $"{responseContent["id"]}";

            // Sprawdzamy, czy ID karty jest niepuste
            Assert.That(_boardId, Is.Not.Null.And.Not.Empty, "Card ID is null or empty.");
        }

        [Test, Order(14)]
        public async Task CreateCard_ShouldReturnNotFound()
        {
            string fakeId = BitConverter.ToString(Guid.NewGuid().ToByteArray(), 0, 12).Replace("-", "").ToLower();
            var response = await _client.CreateCard("", "", fakeId);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "Card with non existing list ID was created.");
        }

        [Test, Order(15)]
        public async Task GetCard_ShouldReturnSuccess()
        {
            // Test pobierania karty po jej utworzeniu
            var response = await _client.GetCard(_cardId);

            // Sprawdzamy, czy odpowiedź jest poprawna i nie pusta
            Assert.Multiple(() =>
            {
                Assert.That(response.IsSuccessful, Is.True, "Card retrive failed.");
                Assert.That(response.Content, Is.Not.Null.And.Not.Empty, "There is no response.");
            });

            // Parsowanie odpowiedzi JSON i porównanie ID karty
            var responseContent = JObject.Parse(response.Content);
            string cardId = $"{responseContent["id"]}";
            Assert.That(cardId, Is.EqualTo(_cardId), "Card retrive failed. Ids does not match.");
        }

        [Test, Order(16)]
        public async Task GetCard_ShouldReturnNotFound()
        {
            // Test próby pobrania karty o nieistniejącym ID
            string fakeId = BitConverter.ToString(Guid.NewGuid().ToByteArray(), 0, 12).Replace("-", "").ToLower();
            var response = await _client.GetCard(fakeId);

            // Sprawdzamy, czy odpowiedź zawiera kod błędu 404 (NotFound)
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "Card with non existen ID was returned.");
        }

        [Test, Order(17)]
        public async Task UpdateCard_ShouldReturnSuccess()
        {
            // Test aktualizacji tytułu oraz opisu karty
            string newName = "Updated card";
            string newDesc = "Updated description";
            var response = await _client.UpdateCard(_cardId, newName, newDesc);

            // Sprawdzamy, czy odpowiedź jest poprawna
            Assert.That(response.IsSuccessful, Is.True, "Card update failed.");

            // Pobieramy zaktualizowaną kartę i sprawdzamy poprawność
            response = await _client.GetCard(_cardId);
            Assert.That(response.Content, Is.Not.Null.And.Not.Empty, "There is no response.");
            var responseContent = JObject.Parse(response.Content);
            string cardId = $"{responseContent["id"]}";
            string cardName = $"{responseContent["name"]}";
            string cardDesc = $"{responseContent["desc"]}";

            // Sprawdzamy, czy ID karty się zgadza, tytuł i opis został zaktualizowany
            Assert.Multiple(() =>
            {
                Assert.That(cardId, Is.EqualTo(_cardId), "Board update failed. Ids does not match.");
                Assert.That(newName, Is.EqualTo(cardName), "Board update failed. Name does not match.");
                Assert.That(cardDesc, Is.EqualTo(newDesc), "Board update failed. Description does not match.");
            });
        }

        [Test, Order(18)]
        public async Task CreateCard_ShouldReturnUnauthorized()
        {
            // Test próby utworzenia karty bez autoryzacji
            var response = await _nonAuthorizedClient.CreateCard("New card", "New description", _listId);

            // Sprawdzamy, czy odpowiedź zawiera kod błędu 401 (Unauthorized)
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized), "Card created but unauthorized.");
        }

        [Test, Order(19)]
        public async Task DeleteBoard_ShouldReturnUnauthorized()
        {
            // Test próby usunięcia tablicy bez autoryzacji
            var response = await _nonAuthorizedClient.DeleteBoard(_boardId);

            // Sprawdzamy, czy odpowiedź zawiera kod błędu 401 (Unauthorized)
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized), "Board deleted but unauthorized.");
        }

        [Test, Order(20)]
        public async Task DeleteBoard_ShouldReturnSuccess()
        {
            // Test próby usunięcia tablicy
            var response = await _client.DeleteBoard(_boardId);

            // Sprawdzamy, czy odpowiedź jest poprawna
            Assert.That(response.IsSuccessful, Is.True, "Board update failed.");

            // Próbujemy pobrać jeszcze raz wcześniej usuniętą tablicę
            response = await _client.GetBoard(_boardId);

            // Sprawdzamy, czy odpowiedź zawiera kod błędu 404 (NotFound)
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound), "Board still exists after deletion.");
        }
    }
}