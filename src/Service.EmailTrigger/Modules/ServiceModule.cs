using Autofac;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using MyServiceBus.TcpClient;
using Service.EmailSender.Client;
using Service.EmailTrigger.Jobs;
using Service.PasswordRecovery.Domain.Models;

namespace Service.EmailTrigger.Modules
{
	public class ServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			const string queueName = "MyJetEducation-EmailTrigger";

			MyServiceBusTcpClient serviceBusClient = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.ServiceBusReader), Program.LogFactory);
			builder.RegisterMyServiceBusSubscriberBatch<RecoveryInfoServiceBusModel>(serviceBusClient, RecoveryInfoServiceBusModel.TopicName, queueName, TopicQueueType.Permanent);

			builder.RegisterEmailSenderClient(Program.Settings.EmailSenderGrpcServiceUrl);

			builder
				.RegisterType<EmailNotificator>()
				.AutoActivate()
				.SingleInstance();
		}
	}
}