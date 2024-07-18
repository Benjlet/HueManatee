using HueManatee.Abstractions;
using HueManatee.Json;
using HueManatee.Request;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HueManatee.Tests
{
    public class HueManateeTests
    {
        private Mock<IHttpClientWrapper> _mockHttpClientWrapper;

        [SetUp]
        public void Setup()
        {
            _mockHttpClientWrapper = new Mock<IHttpClientWrapper>();
        }

        [Test]
        public async Task Register_ShouldMapAndReturnDetails()
        {
            string expectedUsername = "us3rn4m3str1ngc0nt41n1ngth3us3rn4m3";
            string expectedError = "your device fell off the wall";

            _mockHttpClientWrapper.Setup(x => x.PostAsync<List<HueRegisterResult>>(It.IsAny<string>(), It.IsAny<HueRegisterRequest>())).ReturnsAsync(
            [
                new HueRegisterResult()
                {
                    Success = new HueRegisterSuccess()
                    {
                        UserName = expectedUsername
                    }
                },
                new HueRegisterResult()
                {
                    Error = new HueError()
                    {
                        Address = "/",
                        Description = expectedError,
                        Type = 101
                    }
                }
            ]);

            BridgeRegistrationClient hueManatee = new(_mockHttpClientWrapper.Object);
            Response.RegisterResponse response = await hueManatee.Register(new RegisterRequest("Test"));

            _mockHttpClientWrapper.Verify(x => x.PostAsync<List<HueRegisterResult>>(It.IsAny<string>(), It.IsAny<HueRegisterRequest>()), Times.Once());

            Assert.That(response.Errors.Count() == 1, Is.True);
            Assert.That(response.Errors.First() == expectedError, Is.True);
            Assert.That(response.UserName == expectedUsername, Is.True);
        }

        [Test]
        public async Task GetLights_ShouldMapAndReturnDetails()
        {
            _mockHttpClientWrapper.Setup(x => x.GetAsync<Dictionary<string, HueLight>>(It.IsAny<string>())).ReturnsAsync(new Dictionary<string, HueLight>()
            {
                {
                    "1", new HueLight()
                    {
                        Name = "main light",
                        State = new HueLightState()
                        {
                            On = true,
                            Brightness = 100
                        }
                    }
                },
                {
                    "2", new HueLight()
                    {
                        Name = "main light 2",
                        State = new HueLightState()
                        {
                            On = false,
                            Brightness = 220
                        }
                    }
                }
            });

            BridgeClient hueManatee = new(_mockHttpClientWrapper.Object);

            IEnumerable<Response.Light> response = await hueManatee.GetLightData();

            List<Response.Light> lights = response.ToList();

            _mockHttpClientWrapper.Verify(x => x.GetAsync<Dictionary<string, HueLight>>(It.IsAny<string>()), Times.Once());

            Assert.That(lights.Count == 2, Is.True);
            Assert.That(lights[0].Id == "1", Is.True);
            Assert.That(lights[1].Id == "2", Is.True);
            Assert.That(lights[0].Name == "main light", Is.True);
            Assert.That(lights[1].Name == "main light 2", Is.True);
            Assert.That(lights[0].State.Brightness == 100, Is.True);
            Assert.That(lights[1].State.Brightness == 220, Is.True);
            Assert.That(lights[0].State.On, Is.True);
            Assert.That(lights[1].State.On, Is.False);
        }

        [Test]
        public async Task GetGroups_ShouldMapAndReturnDetails()
        {
            _mockHttpClientWrapper.Setup(x => x.GetAsync<Dictionary<string, HueGroup>>(It.IsAny<string>())).ReturnsAsync(new Dictionary<string, HueGroup>()
            {
                {
                    "1", new HueGroup()
                    {
                        Name = "Group 1",
                        Lights = ["1", "2"],
                        Sensors = [],
                        Type = "Zone"
                    }
                },
                {
                    "2", new HueGroup()
                    {
                        Name = "Group 2",
                        Lights = [],
                        Sensors = [],
                        Type = "Room"
                    }
                }
            });

            BridgeClient hueManatee = new(_mockHttpClientWrapper.Object);

            IEnumerable<Response.Group> response = await hueManatee.GetGroupData();

            List<Response.Group> groups = response.ToList();

            _mockHttpClientWrapper.Verify(x => x.GetAsync<Dictionary<string, HueGroup>>(It.IsAny<string>()), Times.Once());

            Assert.That(groups.Count == 2, Is.True);
            Assert.That(groups[0].Id == "1", Is.True);
            Assert.That(groups[1].Id == "2", Is.True);

            Assert.That(groups[0].Type == "Zone", Is.True);
            Assert.That(groups[1].Type == "Room", Is.True);

            Assert.That(groups[0].Name == "Group 1", Is.True);
            Assert.That(groups[1].Name == "Group 2", Is.True);

            Assert.That(groups[0].Lights.Count() == 2, Is.True);
            Assert.That(groups[1].Lights.Any(), Is.False);

            Assert.That(groups[0].Sensors.Any(), Is.False);
            Assert.That(groups[1].Sensors.Any(), Is.False);

        }

        [Test]
        public async Task GetGroupData_ShouldMapAndReturnResult()
        {
            _mockHttpClientWrapper.Setup(x => x.GetAsync<HueGroup>(It.IsAny<string>())).ReturnsAsync(new HueGroup()
            {
                Lights = ["1", "2", "3"],
                Sensors = ["4", "5"],
                Name = "Example group",
                Type = "Zone"
            });

            BridgeClient hueManatee = new(_mockHttpClientWrapper.Object);

            Response.Group light = await hueManatee.GetGroupData("1");

            _mockHttpClientWrapper.Verify(x => x.GetAsync<HueGroup>(It.IsAny<string>()), Times.Once());

            Assert.That(light, Is.Not.Null);
            Assert.That(light.Type == "Zone", Is.True);
            Assert.That(light.Lights.Count() == 3, Is.True);
            Assert.That(light.Sensors.Count() == 2, Is.True);
            Assert.That(light.Name == "Example group", Is.True);
        }

        [Test]
        public async Task GetLightData_ShouldMapAndReturnResult()
        {
            _mockHttpClientWrapper.Setup(x => x.GetAsync<HueLight>(It.IsAny<string>())).ReturnsAsync(new HueLight()
            {
                Name = "main light",
                State = new HueLightState()
                {
                    On = true,
                    Brightness = 100
                }
            });

            BridgeClient hueManatee = new(_mockHttpClientWrapper.Object);

            Response.Light light = await hueManatee.GetLightData("1");

            _mockHttpClientWrapper.Verify(x => x.GetAsync<HueLight>(It.IsAny<string>()), Times.Once());

            Assert.That(light, Is.Not.Null);
            Assert.That(light.Name == "main light", Is.True);
            Assert.That(light.State.Brightness == 100, Is.True);
            Assert.That(light.State.On, Is.True);
        }

        [Test]
        public async Task ChangeLights_ShouldMapAndReturnResult()
        {
            _mockHttpClientWrapper.Setup(x => x.PutAsync<List<HueLightUpdateResult>>(It.IsAny<string>(), It.IsAny<HueStateRequest>())).ReturnsAsync(
            [
                new HueLightUpdateResult()
                {
                    Success = new Dictionary<string, string>()
                    {
                        { "/state/on", "true" }
                    }
                },
                new HueLightUpdateResult()
                {
                    Error = new HueError()
                    {
                        Address = "/",
                        Description = "Unrelated error",
                        Type = 102
                    }
                }
            ]);

            BridgeClient hueManatee = new(_mockHttpClientWrapper.Object);

            Response.ChangeLightResponse changeResult = await hueManatee.ChangeLight("1", new ChangeLightRequest()
            {
                On = true,
                Brightness = 100
            });

            _mockHttpClientWrapper.Verify(x => x.PutAsync<List<HueLightUpdateResult>>(It.IsAny<string>(), It.IsAny<HueStateRequest>()), Times.Once());

            Assert.That(changeResult, Is.Not.Null);
            Assert.That(changeResult.Changes.First().Value == "true", Is.True);
            Assert.That(changeResult.Errors.First() == "Unrelated error", Is.True);
        }

        [Test]
        public async Task ChangeLightColor_ShouldMapAndReturnResult()
        {
            _mockHttpClientWrapper.Setup(x => x.PutAsync<List<HueLightUpdateResult>>(It.IsAny<string>(), It.IsAny<HueStateRequest>())).ReturnsAsync(
            [
                new HueLightUpdateResult()
                {
                    Success = new Dictionary<string, string>()
                    {
                        { "/lights/3/state/hue", "31674" },
                        { "/lights/3/state/sat", "182" },
                        { "/lights/3/state/bri", "224" }
                    }
                },
                new HueLightUpdateResult()
                {
                    Error = new HueError()
                    {
                        Address = "/",
                        Description = "Unrelated error",
                        Type = 102
                    }
                }
            ]);

            System.Drawing.Color requestedColor = System.Drawing.Color.Turquoise;
            BridgeClient hueManatee = new(_mockHttpClientWrapper.Object);

            Response.ChangeLightResponse changeResult = await hueManatee.ChangeLight("1", new ChangeLightRequest()
            {
                On = true,
                Color = requestedColor
            });

            _mockHttpClientWrapper.Verify(x => x.PutAsync<List<HueLightUpdateResult>>(It.IsAny<string>(), It.Is<HueStateRequest>(x =>
                x.Brightness == 224 && x.Hue == 31674 && x.Saturation == 182)), Times.Once());

            Assert.That(changeResult, Is.Not.Null);
            Assert.That(changeResult.Changes.First().Value == "31674", Is.True);
            Assert.That(changeResult.Errors.First() == "Unrelated error", Is.True);
        }

        [Test]
        public async Task ChangeLightColor_BrightnessOverride_ShouldMapAndReturnResult()
        {
            int forcedBrightness = 100;

            _mockHttpClientWrapper.Setup(x => x.PutAsync<List<HueLightUpdateResult>>(It.IsAny<string>(), It.IsAny<HueStateRequest>())).ReturnsAsync(
            [
                new HueLightUpdateResult()
                {
                    Success = new Dictionary<string, string>()
                    {
                        { "/lights/3/state/hue", "31674" },
                        { "/lights/3/state/sat", "182" },
                        { "/lights/3/state/bri", "100" }
                    }
                },
                new HueLightUpdateResult()
                {
                    Error = new HueError()
                    {
                        Address = "/",
                        Description = "Unrelated error",
                        Type = 102
                    }
                }
            ]);

            System.Drawing.Color requestedColor = System.Drawing.Color.Turquoise;
            BridgeClient hueManatee = new(_mockHttpClientWrapper.Object);

            Response.ChangeLightResponse changeResult = await hueManatee.ChangeLight("1", new ChangeLightRequest()
            {
                On = true,
                Color = requestedColor,
                Brightness = forcedBrightness
            });

            _mockHttpClientWrapper.Verify(x => x.PutAsync<List<HueLightUpdateResult>>(It.IsAny<string>(), It.Is<HueStateRequest>(x =>
                x.Brightness == forcedBrightness && x.Hue == 31674 && x.Saturation == 182)), Times.Once());

            Assert.That(changeResult, Is.Not.Null);
            Assert.That(changeResult.Changes.First().Value == "31674", Is.True);
            Assert.That(changeResult.Errors.First() == "Unrelated error", Is.True);
        }
    }
}