// See https://aka.ms/new-console-template for more information

using Grpc.Net.Client;

var channel1 = GrpcChannel.ForAddress("https://localhost:8000/service1/");
var client1 = new GrpcService1.Greeter.GreeterClient(channel1);
var request1 = new GrpcService1.HelloRequest { Name = "Jose Felix from service 1" };
var response1 = client1.SayHello(request1);

Console.WriteLine(response1.Message);

var channel2 = GrpcChannel.ForAddress("https://localhost:8000/service2/");
var client2 = new GrpcService2.Greeter.GreeterClient(channel2);
var request2 = new GrpcService2.HelloRequest { Name = "Jose Felix from service 2" };
var response2 = client2.SayHello(request2);
Console.WriteLine(response2.Message);

Console.ReadKey();

