﻿using System;
using System.Collections.Generic;
using System.Linq;
using Exceptionless.Core.Plugins.EventProcessor;
using Exceptionless.Core.Repositories;

namespace Exceptionless.Core.Pipeline {
    [Priority(40)]
    public class SaveEventAction : EventPipelineActionBase {
        private readonly IEventRepository _eventRepository;

        public SaveEventAction(IEventRepository eventRepository) {
            _eventRepository = eventRepository;
        }

        protected override bool IsCritical { get { return true; } }

        public override void ProcessBatch(ICollection<EventContext> contexts) {
            try {
                _eventRepository.Add(contexts.Select(c => c.Event).ToList());
            } catch (Exception ex) {
                foreach (var context in contexts) {
                    bool cont = false;
                    try {
                        cont = HandleError(ex, context);
                    } catch {}

                    if (!cont)
                        context.SetError(ex.Message, ex);
                }
            }
        }

        public override void Process(EventContext ctx) {}
    }
}