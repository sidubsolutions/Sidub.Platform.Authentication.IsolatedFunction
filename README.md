# Sidub Platform - Authentication - Isolated Function

This repository contains the Isolated Function authentication library for the
Sidub Platform. It supports authentication capabilities when developing
Azure Isolated Functions.

## Main Components
To leverage the authorization framework, it must be first registered
within the function. To do this, simply call the 
`AddSidubAuthenticationForIsolatedFunction` method on the 
`IFunctionsWorkerApplicationBuilder` instance.

```csharp
var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults((context, builder) =>
    {
        builder.AddSidubAuthenticationForIsolatedFunction();

    })
    .ConfigureServices((context, services) =>
    {

    })
    .Build();

host.Run();
```

## License
This project is dual-licensed under the AGPL v3 or a proprietary license. For
details, see [https://sidub.ca/licensing](https://sidub.ca/licensing) or the 
LICENSE.txt file.
