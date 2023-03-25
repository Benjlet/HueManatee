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
            var expectedUsername = "us3rn4m3str1ngc0nt41n1ngth3us3rn4m3";
            var expectedError = "your device fell off the wall";

            _mockHttpClientWrapper.Setup(x => x.PostAsync<List<HueRegisterResult>>(It.IsAny<string>(), It.IsAny<HueRegisterRequest>())).ReturnsAsync(new List<HueRegisterResult>()
            {
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
            });

            var hueManatee = new BridgeRegistrationClient(_mockHttpClientWrapper.Object);
            var response = await hueManatee.Register(new RegisterRequest("Test"));

            _mockHttpClientWrapper.Verify(x => x.PostAsync<List<HueRegisterResult>>(It.IsAny<string>(), It.IsAny<HueRegisterRequest>()), Times.Once());

            Assert.IsTrue(response.Errors.Count() == 1);
            Assert.IsTrue(response.Errors.First() == expectedError);
            Assert.IsTrue(response.UserName == expectedUsername);
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

            var hueManatee = new BridgeClient(_mockHttpClientWrapper.Object);

            var response = await hueManatee.GetLightData();

            var lights = response.ToList();

            _mockHttpClientWrapper.Verify(x => x.GetAsync<Dictionary<string, HueLight>>(It.IsAny<string>()), Times.Once());

            Assert.IsTrue(lights.Count == 2);
            Assert.IsTrue(lights[0].Id == "1");
            Assert.IsTrue(lights[1].Id == "2");
            Assert.IsTrue(lights[0].Name == "main light");
            Assert.IsTrue(lights[1].Name == "main light 2");
            Assert.IsTrue(lights[0].State.Brightness == 100);
            Assert.IsTrue(lights[1].State.Brightness == 220);
            Assert.IsTrue(lights[0].State.On);
            Assert.IsFalse(lights[1].State.On);
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
                        Lights = new List<string>() { "1", "2" },
                        Sensors = new List<string>(),
                        Type = "Zone"
                    }
                },
                {
                    "2", new HueGroup()
                    {
                        Name = "Group 2",
                        Lights = new List<string>(),
                        Sensors = new List<string>(),
                        Type = "Room"
                    }
                }
            });

            var hueManatee = new BridgeClient(_mockHttpClientWrapper.Object);

            var response = await hueManatee.GetGroupData();

            var groups = response.ToList();

            _mockHttpClientWrapper.Verify(x => x.GetAsync<Dictionary<string, HueGroup>>(It.IsAny<string>()), Times.Once());

            Assert.IsTrue(groups.Count == 2);
            Assert.IsTrue(groups[0].Id == "1");
            Assert.IsTrue(groups[1].Id == "2");

            Assert.IsTrue(groups[0].Type == "Zone");
            Assert.IsTrue(groups[1].Type == "Room");

            Assert.IsTrue(groups[0].Name == "Group 1");
            Assert.IsTrue(groups[1].Name == "Group 2");

            Assert.IsTrue(groups[0].Lights.Count() == 2);
            Assert.IsFalse(groups[1].Lights.Any());

            Assert.IsFalse(groups[0].Sensors.Any());
            Assert.IsFalse(groups[1].Sensors.Any());

        }

        [Test]
        public async Task GetGroupData_ShouldMapAndReturnResult()
        {
            _mockHttpClientWrapper.Setup(x => x.GetAsync<HueGroup>(It.IsAny<string>())).ReturnsAsync(new HueGroup()
            {
                Lights = new List<string>() { "1", "2", "3" },
                Sensors = new List<string>() { "4", "5" },
                Name = "Example group",
                Type = "Zone"
            });

            var hueManatee = new BridgeClient(_mockHttpClientWrapper.Object);

            var light = await hueManatee.GetGroupData("1");

            _mockHttpClientWrapper.Verify(x => x.GetAsync<HueGroup>(It.IsAny<string>()), Times.Once());

            Assert.IsNotNull(light);
            Assert.IsTrue(light.Type == "Zone");
            Assert.IsTrue(light.Lights.Count() == 3);
            Assert.IsTrue(light.Sensors.Count() == 2);
            Assert.IsTrue(light.Name == "Example group");
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

            var hueManatee = new BridgeClient(_mockHttpClientWrapper.Object);

            var light = await hueManatee.GetLightData("1");

            _mockHttpClientWrapper.Verify(x => x.GetAsync<HueLight>(It.IsAny<string>()), Times.Once());

            Assert.IsNotNull(light);
            Assert.IsTrue(light.Name == "main light");
            Assert.IsTrue(light.State.Brightness == 100);
            Assert.IsTrue(light.State.On);
        }

        [Test]
        public async Task ChangeLights_ShouldMapAndReturnResult()
        {
            _mockHttpClientWrapper.Setup(x => x.PutAsync<List<HueLightUpdateResult>>(It.IsAny<string>(), It.IsAny<HueStateRequest>())).ReturnsAsync(new List<HueLightUpdateResult>()
            {
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
            });

            var hueManatee = new BridgeClient(_mockHttpClientWrapper.Object);

            var changeResult = await hueManatee.ChangeLight("1", new ChangeLightRequest()
            {
                On = true,
                Brightness = 100
            });

            _mockHttpClientWrapper.Verify(x => x.PutAsync<List<HueLightUpdateResult>>(It.IsAny<string>(), It.IsAny<HueStateRequest>()), Times.Once());

            Assert.IsNotNull(changeResult);
            Assert.IsTrue(changeResult.Changes.First().Value == "true");
            Assert.IsTrue(changeResult.Errors.First() == "Unrelated error");
        }

        [Test]
        public async Task ChangeLightColor_ShouldMapAndReturnResult()
        {
            _mockHttpClientWrapper.Setup(x => x.PutAsync<List<HueLightUpdateResult>>(It.IsAny<string>(), It.IsAny<HueStateRequest>())).ReturnsAsync(new List<HueLightUpdateResult>()
            {
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
            });

            var requestedColor = System.Drawing.Color.Turquoise;
            var hueManatee = new BridgeClient(_mockHttpClientWrapper.Object);

            var changeResult = await hueManatee.ChangeLight("1", new ChangeLightRequest()
            {
                On = true,
                Color = requestedColor
            });

            _mockHttpClientWrapper.Verify(x => x.PutAsync<List<HueLightUpdateResult>>(It.IsAny<string>(), It.Is<HueStateRequest>(x =>
                x.Brightness == 224 && x.Hue == 31674 && x.Saturation == 182)), Times.Once());

            Assert.IsNotNull(changeResult);
            Assert.IsTrue(changeResult.Changes.First().Value == "31674");
            Assert.IsTrue(changeResult.Errors.First() == "Unrelated error");
        }

        [Test]
        public async Task ChangeLightColor_BrightnessOverride_ShouldMapAndReturnResult()
        {
            var forcedBrightness = 100;

            _mockHttpClientWrapper.Setup(x => x.PutAsync<List<HueLightUpdateResult>>(It.IsAny<string>(), It.IsAny<HueStateRequest>())).ReturnsAsync(new List<HueLightUpdateResult>()
            {
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
            });

            var requestedColor = System.Drawing.Color.Turquoise;
            var hueManatee = new BridgeClient(_mockHttpClientWrapper.Object);

            var changeResult = await hueManatee.ChangeLight("1", new ChangeLightRequest()
            {
                On = true,
                Color = requestedColor,
                Brightness = forcedBrightness
            });

            _mockHttpClientWrapper.Verify(x => x.PutAsync<List<HueLightUpdateResult>>(It.IsAny<string>(), It.Is<HueStateRequest>(x =>
                x.Brightness == forcedBrightness && x.Hue == 31674 && x.Saturation == 182)), Times.Once());

            Assert.IsNotNull(changeResult);
            Assert.IsTrue(changeResult.Changes.First().Value == "31674");
            Assert.IsTrue(changeResult.Errors.First() == "Unrelated error");
        }
    }
}