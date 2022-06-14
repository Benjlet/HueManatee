using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HueManatee.Tests
{
    public class HueManateeTests
    {
        private Mock<IHttpClientFactory> _mockHttpClientFactory;
        private HttpResponseMessage _httpResponse;

        [SetUp]
        public void Setup()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            
            var _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => _httpResponse);

            var _httpClient = new HttpClient(_mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("https://192.168.0.1")
            };

            _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(_httpClient);
        }

        [Test]
        public async Task Register_ShouldMapAndReturnDetails()
        {
            string newUsernameJson = "[{\"success\":{\"username\":\"us3rn4m3str1ngc0nt41n1ngth3us3rn4m3\"}}, {\"error\":{\"type\":101,\"address\":\"/\",\"description\":\"your device fell off the wall.\"}}]";

            _httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(newUsernameJson)
            };

            var hueManatee = new HueManatee(_mockHttpClientFactory.Object);

            var response = await hueManatee.Register(new RegisterRequest()
            {
                DeviceType = "Test"
            });

            Assert.IsTrue(response.Errors.Count == 1);
            Assert.IsNotEmpty(response.Errors.FirstOrDefault());
            Assert.AreEqual(response.UserName, newUsernameJson);
        }

        [Test]
        public async Task GetLightData_ShouldMapAndReturnDetails()
        {
            string lightJson = 
                "{\"state\":{\"on\":true,\"bri\":50,\"hue\":54611,\"sat\":254,\"effect\":\"none\",\"xy\":[0.3724,0.1537],\"ct\":236,\"alert\":\"select\",\"colormode\":\"hs\",\"mode\":\"homeautomation\",\"reachable\":true}," +
                "\"swupdate\":{\"state\":\"noupdates\",\"lastinstall\":\"2022-06-06T13:37:46\"},\"type\":\"Extended color light\",\"name\":\"Main light\",\"modelid\":\"LCA001\",\"manufacturername\":\"Signify Netherlands B.V.\"," +
                "\"productname\":\"Hue color lamp\",\"capabilities\":{\"certified\":true,\"control\":{\"mindimlevel\":200,\"maxlumen\":800,\"colorgamuttype\":\"C\",\"colorgamut\":[[0.6915,0.3083],[0.1700,0.7000],[0.1532,0.0475]]," +
                "\"ct\":{\"min\":153,\"max\":500}},\"streaming\":{\"renderer\":true,\"proxy\":true}},\"config\":{\"archetype\":\"sultanbulb\",\"function\":\"mixed\",\"direction\":\"omnidirectional\",\"startup\":{\"mode\":\"safety\",\"configured\":true}}," +
                "\"uniqueid\":\"11:11:11:11:22:33:44:ea-0b\",\"swversion\":\"1.93.7\",\"swconfigid\":\"AAABBCCDD\",\"productid\":\"Philips-L1GHT\"}";

            _httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(lightJson)
            };

            var hueManatee = new HueManatee(_mockHttpClientFactory.Object);

            var response = await hueManatee.GetLightData(It.IsAny<string>(), It.IsAny<string>());

            Assert.IsTrue(response.State.On);
            Assert.IsTrue(response.State.Brightness == 50);
            Assert.IsTrue(response.Name == "Main light");
        }

        [Test]
        public async Task GetLights_ShouldMapAndReturnDetails()
        {
            string lightJson =
                "{\"3\":" +
                    "{\"state\":{\"on\":true,\"bri\":100,\"hue\":49990,\"sat\":254,\"effect\":\"none\",\"xy\":[0.2375,0.0884],\"ct\":500,\"alert\":\"select\",\"colormode\":\"hs\",\"mode\":\"homeautomation\",\"reachable\":true},\"swupdate\":{\"state\":\"noupdates\"," +
                    "\"lastinstall\":\"2022-06-06T13:37:46\"},\"type\":\"Extended color light\",\"name\":\"Example light 1\",\"modelid\":\"LCA001\",\"manufacturername\":\"Signify Netherlands B.V.\",\"productname\":\"Hue color lamp\",\"capabilities\":{\"certified\":true," +
                    "\"control\":{\"mindimlevel\":200,\"maxlumen\":800,\"colorgamuttype\":\"C\",\"colorgamut\":[[0.6915,0.3083],[0.1700,0.7000],[0.1532,0.0475]],\"ct\":{\"min\":153,\"max\":500}},\"streaming\":{\"renderer\":true,\"proxy\":true}},\"config\":{\"archetype\":\"sultanbulb\"," +
                    "\"function\":\"mixed\",\"direction\":\"omnidirectional\",\"startup\":{\"mode\":\"safety\",\"configured\":true}},\"uniqueid\":\"11:11:22:33:09:af:22:22-0a\",\"swversion\":\"1.93.7\",\"swconfigid\":\"01F50378\",\"productid\":\"Philips-LCA001-4-A19ECLv6\"}," +
                "\"4\":{\"state\":{\"on\":false,\"bri\":254,\"hue\":63992,\"sat\":254,\"effect\":\"none\",\"xy\":[0.6464,0.2865],\"ct\":500,\"alert\":\"select\",\"colormode\":\"xy\",\"mode\":\"homeautomation\",\"reachable\":true},\"swupdate\":{\"state\":\"noupdates\"," +
                    "\"lastinstall\":\"2022-06-05T21:39:05\"},\"type\":\"Extended color light\",\"name\":\"Example light 2\",\"modelid\":\"LCL001\",\"manufacturername\":\"Signify Netherlands B.V.\",\"productname\":\"Hue lightstrip plus\",\"capabilities\":{\"certified\":true," +
                    "\"control\":{\"mindimlevel\":40,\"maxlumen\":1600,\"colorgamuttype\":\"C\",\"colorgamut\":[[0.6915,0.3083],[0.1700,0.7000],[0.1532,0.0475]],\"ct\":{\"min\":153,\"max\":500}},\"streaming\":{\"renderer\":true,\"proxy\":true}},\"config\":{\"archetype\":\"huelightstrip\"," +
                    "\"function\":\"mixed\",\"direction\":\"omnidirectional\",\"startup\":{\"mode\":\"safety\",\"configured\":true}},\"uniqueid\":\"11:11:22:33:09:af:22:22-0a\",\"swversion\":\"1.93.7\",\"swconfigid\":\"2435DF32\",\"productid\":\"Philips-LCL001-1-LedStripsv4\"}," +
                "\"5\":{\"state\":{\"on\":false,\"bri\":254,\"hue\":63992,\"sat\":254,\"effect\":\"none\",\"xy\":[0.6464,0.2865],\"ct\":500,\"alert\":\"select\",\"colormode\":\"xy\",\"mode\":\"homeautomation\",\"reachable\":true},\"swupdate\":{\"state\":\"noupdates\"," +
                    "\"lastinstall\":\"2022-06-06T13:38:13\"},\"type\":\"Extended color light\",\"name\":\"Example light 3\",\"modelid\":\"LCX001\",\"manufacturername\":\"Signify Netherlands B.V.\",\"productname\":\"Hue play gradient lightstrip\",\"capabilities\":{\"certified\":true," +
                    "\"control\":{\"mindimlevel\":10,\"maxlumen\":1600,\"colorgamuttype\":\"C\",\"colorgamut\":[[0.6915,0.3083],[0.1700,0.7000],[0.1532,0.0475]],\"ct\":{\"min\":153,\"max\":500}},\"streaming\":{\"renderer\":true,\"proxy\":true}},\"config\":{\"archetype\":\"huelightstriptv\"," +
                    "\"function\":\"mixed\",\"direction\":\"omnidirectional\",\"startup\":{\"mode\":\"safety\",\"configured\":true}},\"uniqueid\":\"11:11:22:33:09:af:22:22-0a\",\"swversion\":\"1.97.3\",\"swconfigid\":\"5FB438CB\",\"productid\":\"Philips-LCX001-1-LedStripPXv1\"}}";

            _httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(lightJson)
            };

            var hueManatee = new HueManatee(_mockHttpClientFactory.Object);

            var response = await hueManatee.GetLights(It.IsAny<string>());
            var lights = response.ToList();

            Assert.IsTrue(lights.Count == 3);
            
            Assert.IsTrue(lights[0].Name == "Example light 1");
            Assert.IsTrue(lights[1].Name == "Example light 2");
            Assert.IsTrue(lights[2].Name == "Example light 3");

            Assert.IsTrue(lights[0].State.On);
            Assert.IsFalse(lights[1].State.On);
            Assert.IsFalse(lights[2].State.On);
        }

        [Test]
        public async Task ChangeLights_ShouldMapAndReturnDetails()
        {
            string changeLightJson = "[{\"error\":{\"type\":7,\"address\":\"/lights/3/state\",\"description\":\"a random error\"}},{\"success\":{\"/lights/3/state/on\":true}},{\"success\":{\"/lights/3/state/hue\":49990}},{\"success\":{\"/lights/3/state/sat\":254}},{\"success\":{\"/lights/3/state/bri\":100}}]";

            _httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(changeLightJson)
            };

            var hueManatee = new HueManatee(_mockHttpClientFactory.Object);

            var response = await hueManatee.ChangeLightState(It.IsAny<string>(), It.IsAny<string>(), new LightStateRequest()
            {
                Brightness = 50,
                On = true
            });

            Assert.IsTrue(response.Changes.Count == 4);
            Assert.IsTrue(response.Errors.Count == 1);

            Assert.IsTrue(response.Changes.All(c => !string.IsNullOrWhiteSpace(c.Key) && !string.IsNullOrWhiteSpace(c.Value)));
            Assert.IsNotEmpty(response.Errors.FirstOrDefault());
        }
    }
}