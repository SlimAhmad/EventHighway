// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.ClientV2.SubstrateApp;
using EventHighway.ClientV2.SubstrateApp.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

IServiceCollection services = new ServiceCollection();

services.AddSubstrateApp();

IServiceProvider serviceProvider = services.BuildServiceProvider();

await serviceProvider
    .GetRequiredService<NFlixSample>()
    .RunAsync();
