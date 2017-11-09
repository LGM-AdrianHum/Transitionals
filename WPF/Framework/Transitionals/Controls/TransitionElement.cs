//  _____                     _ _   _                   _     
// /__   \_ __ __ _ _ __  ___(_) |_(_) ___  _ __   __ _| |___ 
//   / /\/ '__/ _` | '_ \/ __| | __| |/ _ \| '_ \ / _` | / __|
//  / /  | | | (_| | | | \__ \ | |_| | (_) | | | | (_| | \__ \
//  \/   |_|  \__,_|_| |_|___/_|\__|_|\___/|_| |_|\__,_|_|___/
//                                                            
// Module   : Transitionals/Transitionals/TransitionElement.cs
// Name     : Adrian Hum - adrianhum 
// Created  : 2017-09-23-11:00 AM
// Modified : 2017-11-10-7:46 AM

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Transitionals.Controls
{
    /// <summary>
    ///     An element that can display exactly one piece of visual content. When the content is changed, a
    ///     transition is used to switch between the old and the new.
    /// </summary>
    [ContentProperty("Content")]
    [ComVisible(false)]
    // QUESTION: Why derive from FrameworkElement instead of ContentControl?
    public class TransitionElement : FrameworkElement
    {
        /// <summary>
        ///     Identifies the <see cref="TransitionBeginning" /> routed event.
        /// </summary>
        public static readonly RoutedEvent TransitionBeginningEvent = EventManager.RegisterRoutedEvent(
            "TransitionBeginning",
            RoutingStrategy.Bubble, // QUESTION: is this the correct strategy?
            typeof(TransitionEventHandler),
            typeof(TransitionElement));

        /// <summary>
        ///     Identifies the <see cref="TransitionEnded" /> routed event.
        /// </summary>
        public static readonly RoutedEvent TransitionEndedEvent = EventManager.RegisterRoutedEvent(
            "TransitionEnded",
            RoutingStrategy.Bubble, // QUESTION: is this the correct strategy?
            typeof(TransitionEventHandler),
            typeof(TransitionElement));

        /// <summary>
        ///     Identifies the <see cref="Content" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content",
                typeof(object),
                typeof(TransitionElement),
                new UIPropertyMetadata(null, OnContentChanged, CoerceContent));

        /// <summary>
        ///     Identifies the <see cref="ContentTemplate" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate",
                typeof(DataTemplate),
                typeof(TransitionElement),
                new UIPropertyMetadata(null, OnContentTemplateChanged));

        /// <summary>
        ///     Identifies the <see cref="ContentTemplateSelector" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateSelectorProperty =
            DependencyProperty.Register("ContentTemplateSelector",
                typeof(DataTemplateSelector),
                typeof(TransitionElement),
                new UIPropertyMetadata(null, OnContentTemplateSelectorChanged));

        /// <summary>
        ///     Identifies the <see cref="TransitionsEnabled" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty TransitionsEnabledProperty =
            DependencyProperty.Register("TransitionsEnabled", typeof(bool), typeof(TransitionElement),
                new FrameworkPropertyMetadata(true, OnTransitionsEnabledChanged));

        private static readonly DependencyPropertyKey IsTransitioningPropertyKey =
            DependencyProperty.RegisterReadOnly("IsTransitioning",
                typeof(bool),
                typeof(TransitionElement),
                new UIPropertyMetadata(false));

        /// <summary>
        ///     Identifies the <see cref="IsTransitioning" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsTransitioningProperty =
            IsTransitioningPropertyKey.DependencyProperty;

        /// <summary>
        ///     Identifies the <see cref="Transition" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty TransitionProperty =
            DependencyProperty.Register("Transition", typeof(Transition), typeof(TransitionElement),
                new UIPropertyMetadata(null, null, CoerceTransition));

        /// <summary>
        ///     Identifies the <see cref="NullContentTemplate" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty NullContentTemplateProperty =
            DependencyProperty.Register(
                "NullContentTemplate",
                typeof(DataTemplate),
                typeof(TransitionElement)
            );

        /// <summary>
        ///     Identifies the <see cref="TransitionSelectorProperty" /> dependency property.
        /// </summary>
        public static readonly DependencyProperty TransitionSelectorProperty =
            DependencyProperty.Register("TransitionSelector", typeof(TransitionSelector), typeof(TransitionElement),
                new UIPropertyMetadata(null));

        private static readonly DataTemplate defaultNullContentTemplate;

        private readonly Grid hiddenGrid;

        private Transition activeTransition;

        /// <summary>
        ///     Initializes the static version of <see cref="TransitionElement" />.
        /// </summary>
        static TransitionElement()
        {
            // TraceSwitches.Transitions.Level = TraceLevel.Verbose;

            defaultNullContentTemplate = new DataTemplate();
            var rectangle = new FrameworkElementFactory(typeof(Rectangle));
            rectangle.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Stretch);
            rectangle.SetValue(VerticalAlignmentProperty, VerticalAlignment.Stretch);
            rectangle.SetValue(Shape.FillProperty,
                SystemColors.WindowBrush /*new TemplateBindingExtension(Control.ForegroundProperty)?*/);
            defaultNullContentTemplate.VisualTree = rectangle;
            defaultNullContentTemplate.Seal();

            NullContentTemplateProperty.OverrideMetadata(typeof(TransitionElement),
                new FrameworkPropertyMetadata(defaultNullContentTemplate));

            ClipToBoundsProperty.OverrideMetadata(typeof(TransitionElement),
                new FrameworkPropertyMetadata(null, CoerceClipToBounds));
        }

        /// <summary>
        ///     Initializes the <see cref="TransitionElement" /> instance.
        /// </summary>
        public TransitionElement()
        {
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - Constructor Entry");
            Children = new UIElementCollection(this, null);

            hiddenGrid = new Grid();
            hiddenGrid.Visibility = Visibility.Hidden;
            Children.Add(hiddenGrid);

            NewContentPresenter = new ContentPresenter();
            Children.Add(NewContentPresenter);

            OldContentPresenter = new ContentPresenter();
        }

        /// <summary>
        ///     Gets or sets the content that is presented in the <see cref="TransitionElement" />. This is a dependency property.
        /// </summary>
        /// <value>
        ///     The content that is presented in the <see cref="TransitionElement" />.
        /// </value>
        /// <remarks>
        ///     If a transition is specified on the <see cref="Transition" /> property, changing the
        ///     value of this property will automatically cause the transition to begin.
        /// </remarks>
        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the data template used to display the content of the <see cref="TransitionElement" />.
        ///     This is a dependency property.
        /// </summary>
        /// <remarks>
        ///     Set this property to a <see cref="DataTemplate" /> to specify the appearance of the
        ///     <see cref="TransitionElement" />.
        ///     For more information on data templates, see
        ///     <see href="http://msdn2.microsoft.com/en-us/library/ms742521.aspx">Data Templating Overview</see>.
        /// </remarks>
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate) GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a template selector that enables an application writer to provide custom template-selection logic.
        ///     This is a dependency property.
        /// </summary>
        public DataTemplateSelector ContentTemplateSelector
        {
            get { return (DataTemplateSelector) GetValue(ContentTemplateSelectorProperty); }
            set { SetValue(ContentTemplateSelectorProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a value that indicats if transitions are enabled. This is a dependency property.
        /// </summary>
        /// <value>
        ///     <c>true</c> if transitions are enabled; otherwise <c>false</c>. The default is <c>true</c>.
        /// </value>
        public bool TransitionsEnabled
        {
            get { return (bool) GetValue(TransitionsEnabledProperty); }
            set { SetValue(TransitionsEnabledProperty, value); }
        }


        /// <summary>
        ///     Gets a value that indicates if the selected transition is currently running. This is a dependency property.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the transition is running; otherwise <c>false</c>.
        /// </value>
        public bool IsTransitioning
        {
            get { return (bool) GetValue(IsTransitioningProperty); }
            private set { SetValue(IsTransitioningPropertyKey, value); }
        }


        /// <summary>
        ///     Gets or sets the currently selected transition. This is a dependency property.
        /// </summary>
        /// <value>
        ///     The currently selected <see cref="Transition" />.
        /// </value>
        /// <remarks>
        ///     This transition will be used to animate between old content and new content
        ///     whenever the value of the <see cref="Content" /> property has changed.
        /// </remarks>
        public Transition Transition
        {
            get { return (Transition) GetValue(TransitionProperty); }
            set { SetValue(TransitionProperty, value); }
        }

        /// <summary>
        ///     Gets or sets the <see cref="DataTemplate" /> that should be displayed whenever the <see cref="Content" /> property
        ///     is set to <see langword="null" />. This is a dependency property.
        /// </summary>
        /// <value>
        ///     A <see cref="DataTemplate" /> to display when no content is available; otherwise <see langword="null" />.
        /// </value>
        /// <remarks>
        ///     The value of the <see cref="TransitionToNullIgnored" /> impacts whether this template is transitioned.
        /// </remarks>
        public DataTemplate NullContentTemplate
        {
            get { return (DataTemplate) GetValue(NullContentTemplateProperty); }
            set { SetValue(NullContentTemplateProperty, value); }
        }

        /// <summary>
        ///     Gets or sets a transition selector that enables an application writer to provide custom transition
        ///     selection logic. This is a dependency property.
        /// </summary>
        public TransitionSelector TransitionSelector
        {
            get { return (TransitionSelector) GetValue(TransitionSelectorProperty); }
            set { SetValue(TransitionSelectorProperty, value); }
        }

        protected override int VisualChildrenCount => Children.Count;

        internal UIElementCollection Children { get; }

        private ContentPresenter OldContentPresenter { get; set; }

        // TODO: May have to make this public because in Acropolis PartPane and TabLayoutPane used it
        private ContentPresenter NewContentPresenter { get; set; }

        /// <summary>
        ///     Occurs when the curent transition is starting.
        /// </summary>
        public event TransitionEventHandler TransitionBeginning
        {
            add { AddHandler(TransitionBeginningEvent, value); }
            remove { RemoveHandler(TransitionBeginningEvent, value); }
        }

        /// <summary>
        ///     Raises the <see cref="TransitionBeginning" /> event.
        /// </summary>
        /// <param name="e">
        ///     A <see cref="TransitionEventArgs" /> that contains the event data.
        /// </param>
        protected virtual void OnTransitionBeginning(TransitionEventArgs e)
        {
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose,
                "TransitionElement - OnTransitionBeginning Entry");
            RaiseEvent(e);
        }

        /// <summary>
        ///     Occurs when the current transition has completed.
        /// </summary>
        public event TransitionEventHandler TransitionEnded
        {
            add { AddHandler(TransitionEndedEvent, value); }
            remove { RemoveHandler(TransitionEndedEvent, value); }
        }

        /// <summary>
        ///     Raises the <see cref="TransitionEnded" /> event.
        /// </summary>
        /// <param name="e">
        ///     A <see cref="TransitionEventArgs" /> containing the event data.
        /// </param>
        protected virtual void OnTransitionEnded(TransitionEventArgs e)
        {
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - OnTransitionEnded Entry");
            RaiseEvent(e);
        }

        // Force clip to be true if the active Transition requires it
        private static object CoerceClipToBounds(object element, object value)
        {
            var te = (TransitionElement) element;
            var clip = (bool) value;
            if (!clip && te.IsTransitioning)
            {
                var transition = te.Transition;
                if (transition.ClipToBounds)
                    return true;
            }
            return value;
        }

        // Don't update direct content until done transitioning
        private static object CoerceContent(object element, object value)
        {
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - CoerceContent Entry");
            var te = element as TransitionElement;
            if (te != null && te.IsTransitioning)
            {
                Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose,
                    "TransitionElement - CoerceContent returning te.CurrentContentPresenter.Content");
                return te.NewContentPresenter.Content;
            }
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose,
                "TransitionElement - CoerceContent returning normal value");
            return value;
        }

        /// <summary>
        ///     Handles a change to the Content property.
        /// </summary>
        private static void OnContentChanged(object element, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - OnContentChanged Entry");
            var te = element as TransitionElement;
            if (te != null)
            {
                var contentPresenter = te.Content as ContentPresenter;
                var parentFE = te.Parent as FrameworkElement;
                Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose,
                    "TransitionElement - OnContentChanged Beginning Transition");
                te.BeginTransition();
            }
        }

        private static void OnContentTemplateChanged(object element, DependencyPropertyChangedEventArgs e)
        {
            var te = (TransitionElement) element;
            te.NewContentPresenter.ContentTemplate = (DataTemplate) e.NewValue;
        }

        private static void OnContentTemplateSelectorChanged(object element, DependencyPropertyChangedEventArgs e)
        {
            var te = (TransitionElement) element;
            te.NewContentPresenter.ContentTemplateSelector = (DataTemplateSelector) e.NewValue;
        }

        private static void OnTransitionsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TransitionElement) d).HandleTransitionsEnabledChanged(e);
        }

        private void HandleTransitionsEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
            // Notify
            OnTransitionsEnabledChanged(e);
        }

        /// <summary>
        ///     Occurs when the value of the <see cref="TransitionsEnabled" /> property has changed.
        /// </summary>
        /// <param name="e">
        ///     A <see cref="DependencyPropertyChangedEventArgs" /> containing event information.
        /// </param>
        protected virtual void OnTransitionsEnabledChanged(DependencyPropertyChangedEventArgs e)
        {
        }

        private static object CoerceTransition(object element, object value)
        {
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - CoerceTransition Entry");
            var te = (TransitionElement) element;
            if (te.IsTransitioning)
            {
                Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose,
                    "TransitionElement - CoerceTransition returning active transition");
                return te.activeTransition;
            }
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose,
                "TransitionElement - CoerceTransition returning normal current transition");
            return value;
        }

        /// <summary>
        ///     Starts the selected <see cref="Transition" />.
        /// </summary>
        public void BeginTransition()
        {
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose, "TransitionElement - BeginTransition Entry");

            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose,
                "TransitionElement - effectiveContent using direct content");
            var newContent = Content;

            var existingContentPresenter = NewContentPresenter;
            var oldContent = existingContentPresenter.Content;

            var transitionSelector = TransitionSelector;

            var transition = Transition;
            if (transitionSelector != null)
                transition = transitionSelector.SelectTransition(oldContent, newContent);

            var transitioningToNullContent = newContent == null;
            var transitioningFromNullContent = oldContent == null;

            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose && transition != null,
                "TransitionElement - BeginTransition transition is set");
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose && transition == null,
                "TransitionElement - BeginTransition transition is null");

            var shouldTransition = transition != null && TransitionsEnabled && !SkipTransition(transition,
                                       existingContentPresenter, transitioningToNullContent,
                                       transitioningFromNullContent);

            if (shouldTransition)
            {
                Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose,
                    "TransitionElement - BeginTransition Swapping content presenters");
                // Swap content presenters
                var temp = OldContentPresenter;
                OldContentPresenter = NewContentPresenter;
                NewContentPresenter = temp;
            }

            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose,
                "TransitionElement - BeginTransition Updating current content presenter's content");
            var newContentPresenter = NewContentPresenter;
            // Set the current content
            newContentPresenter.Content = newContent;
            newContentPresenter.ContentTemplate = ContentTemplate;
            newContentPresenter.ContentTemplateSelector = ContentTemplateSelector;

            if (shouldTransition)
            {
                var oldContentPresenter = OldContentPresenter;

                if (oldContent == null && NullContentTemplate != null)
                    oldContentPresenter.ContentTemplate = NullContentTemplate;
                if (newContent == null && NullContentTemplate != null)
                    newContentPresenter.ContentTemplate = NullContentTemplate;

                if (transition.IsNewContentTopmost)
                    Children.Add(NewContentPresenter);
                else
                    Children.Insert(0, NewContentPresenter);

                Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose,
                    "TransitionElement - BeginTransition Setting up for transition");
                IsTransitioning = true;
                activeTransition = transition;
                CoerceValue(TransitionProperty);
                CoerceValue(ClipToBoundsProperty);
                OnTransitionBeginning(new TransitionEventArgs(TransitionBeginningEvent, this, transition, oldContent,
                    newContent));
                Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose,
                    "TransitionElement - BeginTransition Calling transition's BeginTransition");
                transition.BeginTransition(this, oldContentPresenter, newContentPresenter);
            }
        }

        private bool SkipTransition(Transition transition, ContentPresenter existingContentPresenter,
            bool transitioningToNullContent, bool transitioningFromNullContent)
        {
            Debug.Assert(transition != null);

            if (transitioningToNullContent && (transition.NullContentSupport == NullContentSupport.Old ||
                                               transition.NullContentSupport == NullContentSupport.None))
                if (ContentTemplate == null &&
                    ContentTemplateSelector == null &&
                    NullContentTemplate == null)
                    return true;
            if (transitioningFromNullContent && (transition.NullContentSupport == NullContentSupport.New ||
                                                 transition.NullContentSupport == NullContentSupport.None))
                if (existingContentPresenter.ContentTemplate == null &&
                    existingContentPresenter.ContentTemplateSelector == null &&
                    NullContentTemplate == null)
                    return true;
            return false;
        }

        public void HideContent(ContentPresenter content)
        {
            if (Children.Contains(content))
            {
                Children.Remove(content);
                hiddenGrid.Children.Add(content);
            }
        }

        // Clean up after the transition is complete
        internal void OnTransitionCompleted(Transition transition, object oldContent, object newContent)
        {
            Debug.WriteLineIf(TraceSwitches.Transitions.TraceVerbose,
                "TransitionElement - OnTransitionCompleted Entry");

            // The parameters passed here are what the transition animated (which is a content presenter) 
            // and not the actual content presented.
            var actualOldContent = OldContentPresenter.Content;
            var actualNewContent = NewContentPresenter.Content;

            // If the newContentPresenter has content and the presenter is removed from the child collection, 
            // any VisualBrush targeting the presenter could cause an InvalidOperationException deep in the 
            // core of WPF (NotifyPartitionIsZombie). The easiest way to work around this WPF issue is to 
            // disassociate the presenter with it's content while changing parents in the visual tree.

            // Disassociate content from the presenter
            NewContentPresenter.Content = null;

            // Clear the child collection (transitions may have added other things like a Viewport3D)
            Children.Clear();

            // Clear the hidden grid collection too because we'll reparent anything here on the next transition
            hiddenGrid.Children.Clear();

            // Add the new content presenter and the hidden grid back into the child collectoin
            Children.Add(NewContentPresenter);
            Children.Add(hiddenGrid);

            // Restore the content back into the presenter now that it's safely reparented.
            NewContentPresenter.Content = actualNewContent;

            // Clear out old content. It should no longer be rooted by the transition element.
            OldContentPresenter.Content = null;

            // Done transitioning
            IsTransitioning = false;
            activeTransition = null;

            // Update measurements
            CoerceValue(TransitionProperty);
            CoerceValue(ClipToBoundsProperty);
            CoerceValue(ContentProperty);

            // Notify listeners of completion
            OnTransitionEnded(new TransitionEventArgs(TransitionEndedEvent, this, transition, actualOldContent,
                actualNewContent));
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            NewContentPresenter.Measure(availableSize);
            return NewContentPresenter.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement uie in Children)
                uie.Arrange(new Rect(finalSize));
            return finalSize;
        }

        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId =
            "System.ArgumentException.#ctor(System.String)")]
        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= Children.Count)
                throw new ArgumentOutOfRangeException("index");
            return Children[index];
        }
    }
}