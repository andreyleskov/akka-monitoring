using Akka.Actor;
using Akka.Event;
using Akka.Monitoring.Impl;
using Akka.Util;

namespace Akka.Monitoring
{
    /// <summary>
    /// <see cref="ActorSystem"/> extension that enables the developer to utilize helpful default performance counters for actor systems
    /// (such as the number <see cref="DeadLetter"/> instances) and define application-specific performance counters that might be relevant.
    /// 
    /// Only one instance of the <see cref="ActorMonitor"/> extension can be active for a given <see cref="ActorSystem"/>, but it
    /// can be safely used across all actors at any time.
    /// </summary>
    public class ActorMonitor : IExtension
    {
        /// <summary>
        /// the internal registry used to track individual monitoring instances
        /// </summary>
        internal MonitorRegistry Registry = new MonitorRegistry();

        /// <summary>
        /// The SampleRate used by default across all calls unless otherwise specified
        /// </summary>
        internal double GlobalSampleRate = 1.0d;

        /// <summary>
        /// Register a new <see cref="AbstractActorMonitoringClient"/> instance to use when monitoring Actor operations.
        /// </summary>
        /// <returns>true if the monitor was successfully registered, false otherwise.</returns>
        public bool RegisterMonitor(AbstractActorMonitoringClient client)
        {
            return Registry.AddMonitor(client);
        }

        /// <summary>
        /// De-register an existing <see cref="AbstractActorMonitoringClient"/> instance so it no longer reports metrics to existing Actors.
        /// </summary>
        /// <returns>true if the monitor was successfully de-registered, false otherwise.</returns>
        public bool DeregisterMonitor(AbstractActorMonitoringClient client)
        {
            return Registry.RemoveMonitor(client);
        }

        /// <summary>
        /// Set a global sample rate for all counters
        /// </summary>
        /// <param name="sampleRate"></param>
        public void SetGlobalSampleRate(double sampleRate)
        {
            GlobalSampleRate = sampleRate;
        }

        /// <summary>
        /// Terminates all existing monitors. You can add new ones after this call has been made.
        /// </summary>
        public void TerminateMonitors()
        {
            Registry.DisposeAll();
        }

        /// <summary>
        /// Increment given counter.
        /// </summary>
        /// <param name="defaulCounterName"></param>
        /// <param name="context"></param>
        /// <param name="value"></param>
        /// <param name="sampleRate"></param>
        /// <param name="actorGroup"></param>
        private void IncrementCounter(string defaulCounterName,
                                      IActorContext context = null,
                                      int value = 1,
                                      double? sampleRate = null,
                                      string actorGroup = null)
        {
            var counterActorGroup = actorGroup ??
                                (context == null ? defaulCounterName : CounterNames.ActorSpecificCategory(context, defaulCounterName));

            var counterSampleRate = sampleRate ?? GlobalSampleRate;

            Registry.UpdateCounter(counterActorGroup, value, counterSampleRate);
        }

        /// <summary>
        /// Increment the "Actor Restarts" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        /// <param name="actorGroup">The name of actor group to show in counter. Actor type name by default</param>
        public void IncrementActorRestart(IActorContext context = null, int value = 1, double? sampleRate = null, string actorGroup = null)
        {
            IncrementCounter(CounterNames.ActorRestarts, context, value, sampleRate, actorGroup);
        }


        /// <summary>
        /// Increment the "Actors Created" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        /// <param name="actorGroup">The name of actor group to show in counter. Actor type name by default</param>
        public void IncrementActorCreated(IActorContext context = null, int value = 1, double? sampleRate = null, string actorGroup = null)
        {
            IncrementCounter(CounterNames.ActorsCreated, context, value, sampleRate, actorGroup);
        }

        /// <summary>
        /// Increment the "Actors Stopped" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        /// <param name="actorGroup">The name of actor group to show in counter. Actor type name by default</param>
        public void IncrementActorStopped(IActorContext context = null, int value = 1, double? sampleRate = null, string actorGroup = null)
        {
            IncrementCounter(CounterNames.ActorsStopped, context, value, sampleRate, actorGroup);
        }

        /// <summary>
        /// Increment the "Messages Received" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        /// <param name="actorGroup">The name of actor group to show in counter. Actor type name by default</param>
        public void IncrementMessagesReceived(IActorContext context = null, int value = 1, double? sampleRate = null, string actorGroup = null)
        {
            IncrementCounter(CounterNames.ReceivedMessages, context, value, sampleRate, actorGroup);
        }

        /// <summary>
        /// Increment the "Unhandled Messages Received" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        /// <param name="actorGroup">The name of actor group to show in counter. Actor type name by default</param>
        public void IncrementUnhandledMessage(IActorContext context = null, int value = 1, double? sampleRate = null, string actorGroup = null)
        {
            IncrementCounter(CounterNames.UnhandledMessages, context, value, sampleRate, actorGroup);
        }

        /// <summary>
        /// Increment the "Deadletters" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        /// <param name="actorGroup">The name of actor group to show in counter. Actor type name by default</param>
        public void IncrementDeadLetters(IActorContext context = null, int value = 1, double? sampleRate = null, string actorGroup = null)
        {
            IncrementCounter(CounterNames.DeadLetters, context, value, sampleRate, actorGroup);
        }

        /// <summary>
        /// Increment the "Errors" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        /// <param name="actorGroup">The name of actor group to show in counter. Actor type name by default</param>
        public void IncrementErrorsLogged(IActorContext context = null, int value = 1, double? sampleRate = null, string actorGroup = null)
        {
            IncrementCounter(CounterNames.ErrorMessages, context, value, sampleRate, actorGroup);
        }

        /// <summary>
        /// Increment the "Warnings" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        /// <param name="actorGroup">The name of actor group to show in counter. Actor type name by default</param>
        public void IncrementWarningsLogged(IActorContext context = null, int value = 1, double? sampleRate = null, string actorGroup = null)
        {
            IncrementCounter(CounterNames.WarningMessages, context, value, sampleRate, actorGroup);
        }

        /// <summary>
        /// Increment the "Debugs" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        /// <param name="actorGroup">The name of actor group to show in counter. Actor type name by default</param>
        public void IncrementDebugsLogged(IActorContext context = null, int value = 1, double? sampleRate = null, string actorGroup = null)
        {
            IncrementCounter(CounterNames.DebugMessages, context, value, sampleRate, actorGroup);
        }

        /// <summary>
        /// Increment the "Infos" counter
        /// </summary>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        /// <param name="actorGroup">The name of actor group to show in counter. Actor type name by default</param>
        public void IncrementInfosLogged(IActorContext context = null, int value = 1, double? sampleRate = null, string actorGroup = null)
        {
            IncrementCounter(CounterNames.InfoMessages, context, value, sampleRate, actorGroup);
        }

        /// <summary>
        /// Increment a custom user-defined counter
        /// </summary>
        /// <param name="metricName">The name of the counter as it will appear in your monitoring system</param>
        /// <param name="value">The value of the counter. 1 by default.</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        /// <param name="context">The context of the actor making this call</param>
        public void IncrementCounter(string metricName, int value = 1, double sampleRate = 1, IActorContext context = null)
        {
            IncrementCounter(metricName, context, value, sampleRate);
        }

        /// <summary>
        /// Increment a custom timing, used to measure the elapsed time of something
        /// </summary>
        /// <param name="metricName">The name of the timing as it will appear in your monitoring system</param>
        /// <param name="time">The amount of time that elapsed, in milliseconds</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="actorGroup">The name of actor group to show in counter. Actor type name by default</param>
        public void Timing(string metricName, long time, double sampleRate = 1, IActorContext context = null)
        {
            var finalMetricName = context == null ? metricName : CounterNames.ActorSpecificCategory(context, metricName);
            Registry.UpdateTimer(finalMetricName, time, sampleRate);
        }

        /// <summary>
        /// Increment a custom Gauge, used to measure arbitrary values (such as the size of messages, etc... non-counter measurements)
        /// </summary>
        /// <param name="metricName">The name of the timing as it will appear in your monitoring system</param>
        /// <param name="value">The value of the gauge</param>
        /// <param name="sampleRate">The sample rate. 100% by default.</param>
        /// <param name="context">The context of the actor making this call</param>
        /// <param name="actorGroup">The name of actor group to show in counter. Actor type name by default</param>
        public void Gauge(string metricName, int value = 1, double sampleRate = 1, IActorContext context = null)
        {
            var finalMetricName = (context == null ? metricName : CounterNames.ActorSpecificCategory(context, metricName));
            Registry.UpdateGauge(finalMetricName, value, sampleRate);
        }
    }

    /// <summary>
    /// The extension class registered with an Akka.NET <see cref="ActorSystem"/>
    /// </summary>
    public class ActorMonitoringExtension : ExtensionIdProvider<ActorMonitor>
    { 
        protected static ConcurrentSet<string> ActorSystemsWithLogger = new ConcurrentSet<string>();
        protected static object LoggerLock = new object();

        public override ActorMonitor CreateExtension(ExtendedActorSystem system)
        {
            try
            {
                //ActorSystem does not have a monitor logger yet
                if (!ActorSystemsWithLogger.Contains(system.Name))
                {
                    lock (LoggerLock)
                    {
                        if (ActorSystemsWithLogger.TryAdd(system.Name))
                        {
                            system.ActorOf<AkkaMonitoringLogger>(AkkaMonitoringLogger.LoggerName);
                        }
                    }
                }
                
            }
            catch { }
            return new ActorMonitor();
        }

        #region Static methods

        public static ActorMonitor Monitors(ActorSystem system)
        {
            return system.WithExtension<ActorMonitor, ActorMonitoringExtension>();
        }

        /// <summary>
        /// Register a new <see cref="AbstractActorMonitoringClient"/> instance to use when monitoring Actor operations.
        /// </summary>
        /// <returns>true if the monitor was succeessfully registered, false otherwise.</returns>
        public static bool RegisterMonitor(ActorSystem system, AbstractActorMonitoringClient client)
        {
            return Monitors(system).RegisterMonitor(client);
        }

        /// <summary>
        /// Deregister an existing <see cref="AbstractActorMonitoringClient"/> instance so it no longer reports metrics to existing Actors.
        /// </summary>
        /// <returns>true if the monitor was succeessfully deregistered, false otherwise.</returns>
        public static bool DeregisterMonitor(ActorSystem system, AbstractActorMonitoringClient client)
        {
            return Monitors(system).DeregisterMonitor(client);
        }

        /// <summary>
        /// Terminates all existing monitors. You can add new ones after this call has been made.
        /// </summary>
        public static void TerminateMonitors(ActorSystem system)
        {
            Monitors(system).TerminateMonitors();
        }

        #endregion
    }
}
