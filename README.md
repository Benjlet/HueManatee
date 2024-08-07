# HueManatee

.NET library for Philips light integration via the Hue Bridge.

[![nuget](https://badgen.net/nuget/v/HueManatee?icon=nuget)](https://www.nuget.org/packages/HueManatee)

# Examples

The main `BridgeClient` allows the user to control lights accessible to their Philips Hue Bridge, supplying details like change to their color, brightness, or 'on' state.

## Get Lights

The below code gets basic data on all lights visible to the authenticated user.

```
IBridgeClient bridgeClient = new BridgeClient("https://bridge.ip.address", "userName");
IEnumerable<Light> lights = await bridgeClient.GetLightData();
```

To query a specific light, pass its ID - you can find out what each one is from getting group or 'all lights' data.

```
IBridgeClient bridgeClient = new BridgeClient("https://bridge.ip.address", "userName");
Light specificLight = await bridgeClient.GetLightData("1");
```

## Get Groups

The below code gets basic data on all groups visible to the authenticated user:

```
IBridgeClient bridgeClient = new BridgeClient("https://bridge.ip.address", "userName");
IEnumerable<Group> groups = await bridgeClient.GetGroupData();
```

To query a specific group, pass its ID - you can find out what each one is from the 'all groups' data:

```
IBridgeClient bridgeClient = new BridgeClient("https://bridge.ip.address", "userName");
Group specificGroup = await bridgeClient.GetGroupData("1");
```

## Change Lights

To change multiple lights at once, they must be linked with a group. The below code turns on all lights in Group "1" and sets the color to Crimson:

```
IBridgeClient bridgeClient = new BridgeClient("https://bridge.ip.address", "userName");
ChangeLightResponse response = await bridgeClient.ChangeLightGroup("1", new ChangeLightRequest()
{
    Color = Color.Crimson,
    On = true
});
```

For changing an individual light, the below code turns on Light "1", sets its color to Violet, and its Brightness to 50:

```
IBridgeClient bridgeClient = new BridgeClient("https://bridge.ip.address", "userName");
ChangeLightResponse response = await bridgeClient.ChangeLight("1", new ChangeLightRequest()
{
    Color = Color.Violet,
    Brightness = 50,
    On = true
});
```

# Registration

For querying and changing lights, the device must have a username registered against the Philips Hue Bridge.

## Get a username

A registration request can be made through the `IBridgeRegistrationClient`:

```
IBridgeRegistrationClient registrationClient = new RegistrationClient("https://bridge.ip.address");

RegisterResponse response = await bridgeClient.Register(new RegisterRequest()
{
    DeviceType = "NewUser"
});
```

The response will contain any error messages returned from the Hue Bridge and, if successful, the UserName value. You will likely get a "link button not pressed" error the first time you set up a user, meaning you need to press the link button on the Hue Bridge then repeat the call.









