using Moq;
using Moq.Protected;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

            var hueManatee = new BridgeClient(_mockHttpClientWrapper.Object);
            var response = await hueManatee.Register(new RegisterRequest()
            {
                DeviceType = "Test"
            });

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
            var response = await hueManatee.GetLights(It.IsAny<string>());

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
            var light = await hueManatee.GetLightData(It.IsAny<string>(), It.IsAny<string>());

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
            var changeResult = await hueManatee.ChangeLightState(It.IsAny<string>(), It.IsAny<string>(), new LightChangeRequest()
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
        public async Task TurnLightOn_ShouldMapAndReturnResult()
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
            var changeResult = await hueManatee.TurnLightOn(It.IsAny<string>(), It.IsAny<string>());

            _mockHttpClientWrapper.Verify(x => x.PutAsync<List<HueLightUpdateResult>>(It.IsAny<string>(), It.IsAny<HueStateRequest>()), Times.Once());

            Assert.IsNotNull(changeResult);
            Assert.IsTrue(changeResult.Changes.First().Value == "true");
            Assert.IsTrue(changeResult.Errors.First() == "Unrelated error");
        }

        [Test]
        public async Task TurnLightOff_ShouldMapAndReturnResult()
        {
            _mockHttpClientWrapper.Setup(x => x.PutAsync<List<HueLightUpdateResult>>(It.IsAny<string>(), It.IsAny<HueStateRequest>())).ReturnsAsync(new List<HueLightUpdateResult>()
            {
                new HueLightUpdateResult()
                {
                    Success = new Dictionary<string, string>()
                    {
                        { "/state/on", "false" }
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
            var changeResult = await hueManatee.TurnLightOff(It.IsAny<string>(), It.IsAny<string>());

            _mockHttpClientWrapper.Verify(x => x.PutAsync<List<HueLightUpdateResult>>(It.IsAny<string>(), It.IsAny<HueStateRequest>()), Times.Once());

            Assert.IsNotNull(changeResult);
            Assert.IsTrue(changeResult.Changes.First().Value == "false");
            Assert.IsTrue(changeResult.Errors.First() == "Unrelated error");
        }

        [Test]
        public async Task StartColorLoop_ShouldMapAndReturnResult()
        {
            _mockHttpClientWrapper.Setup(x => x.PutAsync<List<HueLightUpdateResult>>(It.IsAny<string>(), It.IsAny<HueStateRequest>())).ReturnsAsync(new List<HueLightUpdateResult>()
            {
                new HueLightUpdateResult()
                {
                    Success = new Dictionary<string, string>()
                    {
                        { "/state/effect", "colorloop" }
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
            var changeResult = await hueManatee.StartColorLoop(It.IsAny<string>(), It.IsAny<string>());

            _mockHttpClientWrapper.Verify(x => x.PutAsync<List<HueLightUpdateResult>>(It.IsAny<string>(), It.Is<HueStateRequest>(x => x.Effect == "colorloop")), Times.Once());

            Assert.IsNotNull(changeResult);
            Assert.IsTrue(changeResult.Changes.First().Value == "colorloop");
            Assert.IsTrue(changeResult.Errors.First() == "Unrelated error");
        }

        [Test]
        public async Task StopColorLoop_ShouldMapAndReturnResult()
        {
            _mockHttpClientWrapper.Setup(x => x.PutAsync<List<HueLightUpdateResult>>(It.IsAny<string>(), It.IsAny<HueStateRequest>())).ReturnsAsync(new List<HueLightUpdateResult>()
            {
                new HueLightUpdateResult()
                {
                    Success = new Dictionary<string, string>()
                    {
                        { "/state/effect", "none" }
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
            var changeResult = await hueManatee.StopColorLoop(It.IsAny<string>(), It.IsAny<string>());

            _mockHttpClientWrapper.Verify(x => x.PutAsync<List<HueLightUpdateResult>>(It.IsAny<string>(), It.Is<HueStateRequest>(x => x.Effect == "none")), Times.Once());

            Assert.IsNotNull(changeResult);
            Assert.IsTrue(changeResult.Changes.First().Value == "none");
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
                        { "/state/color", "red" }
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

            var requestedColor = System.Drawing.Color.Red;
            var hueManatee = new BridgeClient(_mockHttpClientWrapper.Object);
            var changeResult = await hueManatee.ChangeLightColor(It.IsAny<string>(), It.IsAny<string>(), requestedColor, 100);

            _mockHttpClientWrapper.Verify(x => x.PutAsync<List<HueLightUpdateResult>>(It.IsAny<string>(), It.Is<HueStateRequest>(x => x.Brightness == 100 && x.Hue == new RGB(requestedColor).GetHue())), Times.Once());

            Assert.IsNotNull(changeResult);
            Assert.IsTrue(changeResult.Changes.First().Value == "red");
            Assert.IsTrue(changeResult.Errors.First() == "Unrelated error");
        }
    }
}