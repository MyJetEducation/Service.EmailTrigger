using Autofac;
using MyJetWallet.Sdk.ServiceBus;
using MyServiceBus.Abstractions;
using MyServiceBus.TcpClient;
using Service.EmailSender.Client;
using Service.EmailTrigger.Jobs;
using Service.PasswordRecovery.Domain.Models;
using Service.Registration.Domain.Models;

namespace Service.EmailTrigger.Modules
{
	public class ServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			MyServiceBusTcpClient serviceBusClient = builder.RegisterMyServiceBusTcpClient(Program.ReloadedSettings(e => e.ServiceBusReader), Program.LogFactory);
			
			builder.RegisterMyServiceBusSubscriberBatch<RecoveryInfoServiceBusModel>(serviceBusClient, RecoveryInfoServiceBusModel.TopicName, "MyJetEducation-EmailTrigger-RecoveryInfo", TopicQueueType.PermanentWithSingleConnection);
			builder.RegisterMyServiceBusSubscriberBatch<RegistrationInfoServiceBusModel>(serviceBusClient, RegistrationInfoServiceBusModel.TopicName, "MyJetEducation-EmailTrigger-RegistrationInfo", TopicQueueType.PermanentWithSingleConnection);

			builder.RegisterEmailSenderClient(Program.Settings.EmailSenderGrpcServiceUrl);

			builder
				.RegisterType<EmailNotificator>()
				.AutoActivate()
				.SingleInstance();
		}
	}
}