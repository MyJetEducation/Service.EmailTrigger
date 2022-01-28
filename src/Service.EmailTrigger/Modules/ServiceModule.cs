using Autofac;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using MyServiceBus.TcpClient;
using Service.EmailSender.Client;
using Service.ServiceBus.Models;

namespace Service.EmailTrigger.Modules
{
	public class ServiceModule : Module
	{
		private const string QueueName = "MyJetEducation-EmailTrigger";

		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterEmailSenderClient(Program.Settings.EmailSenderGrpcServiceUrl);

			MyServiceBusTcpClient serviceBusClient = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.ServiceBusReader), Program.LogFactory);
			builder.RegisterMyServiceBusSubscriberBatch<RecoveryInfoServiceBusModel>(serviceBusClient, RecoveryInfoServiceBusModel.TopicName, QueueName, TopicQueueType.Permanent);
			builder.RegisterMyServiceBusSubscriberBatch<RegistrationInfoServiceBusModel>(serviceBusClient, RegistrationInfoServiceBusModel.TopicName, QueueName, TopicQueueType.Permanent);
		}
	}
}