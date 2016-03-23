using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace LibraProgramming.Windows.UI.Xaml.StateTriggers
{
    /// <summary>
    /// 
    /// </summary>
	public enum AggregationType
	{
        /// <summary>
        /// 
        /// </summary>
		All,

        /// <summary>
        /// 
        /// </summary>
        Any,

        /// <summary>
        /// 
        /// </summary>
        OnlyOne
	}

    /// <summary>
    /// 
    /// </summary>
	public class StateTriggerCollection : DependencyObjectCollection
	{
	}

    /// <summary>
    /// 
    /// </summary>
    [ContentProperty(Name = "StateTriggers")]
	public class CompositeStateTrigger : CustomStateTrigger
    {
	    public static readonly DependencyProperty RegistrationTokenProperty;
	    public static readonly DependencyProperty AggregationTypeProperty;

	    public AggregationType AggregationType
	    {
		    get
		    {
			    return (AggregationType) GetValue(AggregationTypeProperty);
		    }
		    set
		    {
			    SetValue(AggregationTypeProperty, value);
		    }
	    }

		public StateTriggerCollection StateTriggers
		{
			get;
		}

		public CompositeStateTrigger()
		{
			StateTriggers = new StateTriggerCollection();
			StateTriggers.VectorChanged += OnStateTriggersCollectionChanged;
		}

	    static CompositeStateTrigger()
	    {
		    RegistrationTokenProperty = DependencyProperty
			    .RegisterAttached(
				    "RegistrationToken",
				    typeof (long),
				    typeof (CompositeStateTrigger),
				    new PropertyMetadata(0)
			    );
		    AggregationTypeProperty = DependencyProperty
			    .Register(
				    "AggregationType",
				    typeof (AggregationType),
				    typeof (CompositeStateTrigger),
				    new PropertyMetadata(AggregationType.All, OnAggregationTypePropertyChanged)
			    );
	    }

	    public static void SetRegistrationToken(StateTriggerBase trigger, long value)
        {
            trigger.SetValue(RegistrationTokenProperty, value);
        }

        public static long GetRegistrationToken(StateTriggerBase trigger)
        {
	        return (long) trigger.GetValue(RegistrationTokenProperty);
        }

	    private IEnumerable<bool> GetTriggerValues()
	    {
		    foreach (var trigger in StateTriggers)
		    {
			    if (trigger is ITriggerValue)
			    {
				    yield return ((ITriggerValue) trigger).IsActive;
			    }
                else if (trigger is AdaptiveTrigger)
                {
                    throw new NotSupportedException();
                }
                else if (trigger is StateTriggerBase)
			    {
				    var value = trigger.GetValue(StateTrigger.IsActiveProperty);

				    if (value is bool)
				    {
					    yield return (bool) value;
				    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }
                else
			    {
				    throw new NotSupportedException();
			    }
		    }
	    }

	    private void EvaluateTriggers()
	    {
		    if (!StateTriggers.Any())
		    {
			    IsActive = false;
                return;
		    }

		    switch (AggregationType)
		    {
				case AggregationType.All:
			    {
				    IsActive = GetTriggerValues().All(value => value);
				    break;
			    }

                case AggregationType.Any:
			    {
				    IsActive = GetTriggerValues().Any(value => value);
				    break;
			    }

                case AggregationType.OnlyOne:
			    {
				    IsActive = GetTriggerValues().Count(value => value) == 1;
				    break;
			    }
		    }
        }

	    private void OnStateTriggersCollectionChanged(IObservableVector<DependencyObject> sender, IVectorChangedEventArgs args)
	    {
		    if (!ReferenceEquals(StateTriggers, sender))
		    {
			    return;
		    }

		    switch (args.CollectionChange)
		    {
				case CollectionChange.ItemChanged:
			    {
				    var trigger = sender[(int) args.Index] as StateTriggerBase;

				    DoStateTriggersChanged(new[] { trigger }, new[] { trigger });

				    break;
			    }

			    case CollectionChange.ItemInserted:
			    {
				    var trigger = sender[(int) args.Index] as StateTriggerBase;

				    DoStateTriggersChanged(Array.Empty<StateTriggerBase>(), new[] { trigger });

				    break;
			    }

			    case CollectionChange.ItemRemoved:
			    {
				    var trigger = sender[(int) args.Index] as StateTriggerBase;

				    DoStateTriggersChanged(new[] { trigger }, Array.Empty<StateTriggerBase>());

				    break;
			    }

                case CollectionChange.Reset:
			    {
				    DoStateTriggersChanged(Array.Empty<StateTriggerBase>(), StateTriggers.Cast<StateTriggerBase>().ToArray());

				    break;
			    }
		    }
        }

	    private void DoStateTriggersChanged(IEnumerable<StateTriggerBase> removed, IEnumerable<StateTriggerBase> inserted)
	    {
		    foreach (var trigger in removed)
		    {
                var triggerValue = trigger as ITriggerValue;

                if (triggerValue != null)
                {
                    triggerValue.IsActiveChanged -= OnTriggerIsActiveChanged;
                }
                else
			    {
				    var token = GetRegistrationToken(trigger);

				    if (0 < token)
				    {
					    trigger.ClearValue(RegistrationTokenProperty);
					    trigger.UnregisterPropertyChangedCallback(StateTrigger.IsActiveProperty, token);
				    }
			    }
		    }

		    foreach (var trigger in inserted)
		    {
                var triggerValue = trigger as ITriggerValue;

                if (triggerValue != null)
                {
                    
                    triggerValue.IsActiveChanged += OnTriggerIsActiveChanged;
                }
                else
                {
	                var token = trigger
		                .RegisterPropertyChangedCallback(
			                StateTrigger.IsActiveProperty,
			                OnTriggerIsActivePropertyChangedCallback
		                );
				    SetRegistrationToken(trigger, token);
			    }
		    }

		    EvaluateTriggers();
	    }

	    private void OnTriggerIsActiveChanged(object sender, EventArgs e)
	    {
		    EvaluateTriggers();
	    }

	    private void OnTriggerIsActivePropertyChangedCallback(DependencyObject sender, DependencyProperty dp)
	    {
		    EvaluateTriggers();
	    }

        private static void OnAggregationTypePropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
	        ((CompositeStateTrigger) source).EvaluateTriggers();
        }
    }
}