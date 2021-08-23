using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Net;

namespace ServeRest
{
    public class Tests
    {
        public RestClient client;
        public RestRequest endpoint;
        public IRestResponse resp;
        public string status;
        public string bearer;

        public RestClient Client(string uri)
        {
            client = new RestClient(uri);
            return client;
        }

        public RestRequest Endpoint(string rota)
        {
            endpoint = new RestRequest(rota);
            return endpoint;
        }

        public RestClient ClientAutenticado(string uri)
        {
            client = Client(uri);

            client.AddDefaultHeader("Authorization", bearer);

            return client;
        }

        public void Get()
        {
            endpoint.Method = Method.GET;
            endpoint.RequestFormat = DataFormat.Json;
        }

        public void Post()
        {
            endpoint.Method = Method.POST;
            endpoint.RequestFormat = DataFormat.Json;
        }

        public void Delete()
        {
            endpoint.Method = Method.DELETE;
            endpoint.RequestFormat = DataFormat.Json;
        }

        public IRestResponse StatusCode(int code)
        {
            resp = client.Execute(endpoint);
            if (resp.IsSuccessful)
            {
                var status = (int)resp.StatusCode;
                Assert.AreEqual(code, status);
            }
            else
            {
                var status = (int)resp.StatusCode;
                var desc = resp.StatusDescription;
                var content = resp.Content;

                Console.WriteLine($"{status} - { desc}");
                Console.WriteLine(content);
                Assert.AreEqual(code, status);
            }
            return resp;
        }

        public void Body_json(string _body)
        {
            endpoint.AddParameter("application/json", _body, ParameterType.RequestBody);
        }

        public void ReturnTextLogin()
        {
            JObject obs = JObject.Parse(resp.Content);
            Console.WriteLine(obs);

            var bearerToken = obs["authorization"].ToString();

            bearer = bearerToken;

        }

        public void ReturnText(int quantidade, string usuario)
        {
            JObject obs = JObject.Parse(resp.Content);
            Console.WriteLine(obs);

            var quantidadeRetorno = int.Parse(obs["quantidade"].ToString());
            Assert.AreEqual(quantidade, quantidadeRetorno);

            var usuarioRetorno = obs["usuarios"][0]["nome"].ToString();
            Assert.AreEqual(usuario, usuarioRetorno);
            
        }


        public void ReturnTextCadastro(string mensagem)
        {
            JObject obs = JObject.Parse(resp.Content);
            Console.WriteLine(obs);

            var mensagemRetorno = obs["message"].ToString();
            Assert.AreEqual(mensagemRetorno, mensagem);
        }

        public void ReturnUsuarioId(string usuario, string email)
        {
            JObject obs = JObject.Parse(resp.Content);
            Console.WriteLine(obs);

            var nomeUsuarioID = obs["nome"].ToString();
            var emailUsuarioID = obs["email"].ToString();

            Assert.AreEqual(usuario, nomeUsuarioID);
            Assert.AreEqual(email, emailUsuarioID);
        }

        public void ReturnUsuarioIdErrado(string mensagem)
        {
            JObject obs = JObject.Parse(resp.Content);
            Console.WriteLine(obs);

            var usuarioNaoEncontrado = obs["message"].ToString();
            Assert.AreEqual(usuarioNaoEncontrado, mensagem);
            
        }

        public void ReturnDeleteUsuario(string mensagem)
        {
            JObject obs = JObject.Parse(resp.Content);
            Console.WriteLine(obs);

            var mensagemRetorno = obs["message"].ToString();
            Assert.AreEqual(mensagemRetorno, mensagem);
        }

        public void ReturnPesquisaProdutos()
        {
            JObject obs = JObject.Parse(resp.Content);
            Console.WriteLine(obs);
        }

        public void ReturnPesquisaProdutosID()
        {
            JObject obs = JObject.Parse(resp.Content);
            Console.WriteLine(obs);
        }

        public void ReturnTextCadastroProduto(string mensagem)
        {
            JObject obs = JObject.Parse(resp.Content);
            Console.WriteLine(obs);

            var mensagemRetorno = obs["message"].ToString();
            Assert.AreEqual(mensagemRetorno, mensagem);
        }

        public string jsonLogin()
        {

            var body = @"{
                          ""email"": ""fulano@qa.com"",
                          ""password"": ""teste""
                        }";
            return body;
        }


        public string json()
        {

            var body = @"{
                          ""nome"": ""GabrielaTeste"",
                          ""email"": ""gabriela09@qa.com.br"",
                          ""password"": ""teste"",
                          ""administrador"": ""true""
                        }";
            return body;
        }

        public string jsonProduto()
        {

            var body = @"{
                          ""nome"": ""Monitor LCD 36"",
                          ""preco"": ""600.00"",
                          ""descricao"": ""teste monitor"",
                          ""quantidade"": ""3""
                        }";
            return body;
        }

        

        [Test]
        public void Login()
        {
            Client("http://localhost:3000/");
            Endpoint("/login");
            Post();
            Body_json(jsonLogin());
            StatusCode(200);
            ReturnTextLogin();
        }

        

        [Test]
        public void PesquisarUsuarios()
        {
            Client("http://localhost:3000/");
            Endpoint("/usuarios");
            Get();

            StatusCode(200);
            ReturnText(4, "Fulano da Silva");
        }

        [Test]
        public void StatusCodeTest()
        {
            // arrange
            RestClient client = new RestClient("http://localhost:3000");
            RestRequest request = new RestRequest("/usuarios", Method.GET);

            // act
            IRestResponse response = client.Execute(request);

            // assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void CadastroUsuario()
        {
            Client("http://localhost:3000/");
            Endpoint("/usuarios");
            Post();
            Body_json(json());
            StatusCode(201);
            ReturnTextCadastro("Cadastro realizado com sucesso");
        }

        [Test]
        public void PesquisarUsuarioId()
        {
            Client("http://localhost:3000");
            Endpoint("/usuarios/36LJXsET0khcNaNs");
            Get();
            StatusCode(200);
            ReturnUsuarioId("GabrielaTeste", "gabriela07@qa.com.br");
        }
        [Test]
        public void PesquisarUsuarioIdErrado()
        {
            Client("http://localhost:3000");
            Endpoint("/usuarios/0uxuPY0cbmQhpEz25");
            Get();
            StatusCode(400);
            ReturnUsuarioIdErrado("Usuário não encontrado");
        }
        [Test]
        public void DeleteUsuario()
        {
            Client("http://localhost:3000");
            Endpoint("/usuarios/0uxuPY0cbmQhpEz1");
            Delete();
            StatusCode(200);
            ReturnDeleteUsuario("Registro excluído com sucesso");
        }

        [Test]
        public void PesquisaProdutos()
        {
            Client("http://localhost:3000/");
            Endpoint("/produtos");
            Get();
            StatusCode(200);
            ReturnPesquisaProdutos();
        }


        [Test]
        public void CadastroProdutos()
        {
            Login();
            ClientAutenticado("http://localhost:3000/");
            Endpoint("/produtos");
            Post();
            Body_json(jsonProduto());
            StatusCode(201);
            ReturnTextCadastro("Cadastro realizado com sucesso");
        }
        [Test]
        public void PesquisaProdutoId()
        {
            Client("http://localhost:3000");
            Endpoint("/produtos/BeeJh5lz3k6kSIzA");
            Get();
            StatusCode(200);
            ReturnPesquisaProdutosID();
        }

        [Test]
        public void PesquisaCarrinhos()

        {
            /*JObject obs = JObject.Parse(resp.Content);
            Console.WriteLine(obs);*/

            // arrange
            RestClient client = new RestClient("http://localhost:3000");
            RestRequest request = new RestRequest("/carrinhos", Method.GET);

            // act
            IRestResponse response = client.Execute(request);


            // assert
            //var mensagemRetorno = obs["message"].ToString();
            //Assert.AreEqual(mensagemRetorno, mensagem);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }



    }
}