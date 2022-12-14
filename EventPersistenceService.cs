using System;
using bagend_ml.Client;
using bagend_ml.Client.Model;

namespace bagend_ml
{
	public class EventPersistenceService
	{
		private readonly EventApiRESTClient _eventApiRESTClient;
        private readonly Executor _executor;
		private readonly ILogger<EventPersistenceService> _logger;

        public EventPersistenceService(EventApiRESTClient eventApiRESTClient,
            Executor executor,
            ILogger<EventPersistenceService> logger)
        {
            _eventApiRESTClient = eventApiRESTClient;
            _executor = executor;
            _logger = logger;
        }

        public void PostRecordedEvent(ExternallyRecordedEvent externallyRecordedEvent)
        {
            var genericEvent = externallyRecordedEvent.GetGenericEvent();
            _logger.LogInformation("submitting externally-recorded-event {} posting to threadpool {}",
                genericEvent.EventName,
                _executor.GetName());
            _executor.execute(new ActionRunnable(() =>
            {
                _eventApiRESTClient.SubmitEvent(genericEvent);
                _logger.LogInformation("externally-recorded-event {} posting thread exiting", genericEvent.EventName);
            }));
        }
    }
}

