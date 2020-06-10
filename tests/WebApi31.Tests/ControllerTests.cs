using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Shared;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi31.Tests
{
    [TestFixture]
    public class ControllerTests
    {
        private BaseWebApplicationFactory<Startup> factory;

        public ControllerTests()
        {
            this.factory = new BaseWebApplicationFactory<Startup>();
        }

        [Test]
        public async Task Cancellation_ShouldReturn499()
        {
            var haxiGuid = new Guid("50d989c9-0a40-4f09-84e8-2bbaa78b3d92");
            var haxi = new Haxi
            {
                Id = haxiGuid
            };
            Func<Guid[], CancellationToken, Task<IList<Haxi>>> mockingFunction = async (_, cancellationToken) =>
            {
                await Task.Delay(2000, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                return new List<Haxi> { haxi };
            };

            var repositoryMock = new Mock<IHaxiRepository>();
            repositoryMock
                .Setup(repo => repo.Get(It.IsAny<Guid[]>(), It.IsAny<CancellationToken>()))
                .Returns(mockingFunction);

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            using var httpMessageHandler = this.CreateHttpMessageInvoker(repositoryMock);
            HttpResponseMessage responseMessage = null;
            await Task.WhenAll(
                Task.Run(async () =>
                    {
                        await Task.Delay(500);
                        tokenSource.Cancel();
                    }),
                Task.Run(async () =>
                    {
                        responseMessage = await httpMessageHandler.SendAsync(
                            new HttpRequestMessage(HttpMethod.Get, new Uri($"http://localhost/api/values/haxi?haxiIds={haxiGuid}")),
                            token);
                    }));
            Assert.That(
                responseMessage.StatusCode,
                Is.EqualTo((HttpStatusCode)499));
        }

        private HttpClient CreateHttpClient(Mock<IHaxiRepository> repositoryMock)
        {
            return this.factory.WithWebHostBuilder(
                builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddScoped(typeof(IHaxiRepository), provider => repositoryMock.Object);
                    });
                }).CreateClient();
        }

        private HttpMessageInvoker CreateHttpMessageInvoker(Mock<IHaxiRepository> repositoryMock)
        {
            return new HttpMessageInvoker(this.factory.WithWebHostBuilder(
                builder =>
                {
                    builder.ConfigureTestServices(services =>
                    {
                        services.AddScoped(typeof(IHaxiRepository), provider => repositoryMock.Object);
                    });
                }).Server.CreateHandler(), true);
        }
    }
}