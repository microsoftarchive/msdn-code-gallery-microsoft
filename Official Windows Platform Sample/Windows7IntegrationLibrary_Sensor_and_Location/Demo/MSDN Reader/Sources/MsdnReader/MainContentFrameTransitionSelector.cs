// Copyright (c) Microsoft Corporation.  All rights reserved.

//---------------------------------------------------------------------------
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//---------------------------------------------------------------------------

namespace MsdnReader
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using Microsoft.SceReader;
    using Microsoft.SceReader.Data;
    using Microsoft.SceReader.View;
    using Microsoft.SceReader.Controls.Transitions;

    public class MsdnReaderTransitionSelector : TransitionSelector
    {
        private delegate Transition SelectorDelegate(object from, object to, SceReaderNavigationMode mode);

        private ICollection<SelectorDelegate> _selectors;

        public MsdnReaderTransitionSelector()
        {
        }

        private ICollection<SelectorDelegate> Selectors
        {
            get
            {
                if (_selectors == null)
                {
                    _selectors = new SelectorDelegate[] { 
                        TransitionNext,
                        TransitionPrevious,
                        TransitionNormal
                    };
                }
                return _selectors;
            }
        }

        private Transition _fallback;
        public Transition Fallback
        {
            get { return _fallback; }
            set { _fallback = value; }
        }

        private Transition _nextTransition;
        public Transition NextTransition
        {
            get { return _nextTransition; }
            set { _nextTransition = value; }
        }

        private Transition _previousTransition;
        public Transition PreviousTransition
        {
            get { return _previousTransition; }
            set { _previousTransition = value; }
        }

        private Transition _normalTransition;
        public Transition NormalTransition
        {
            get { return _normalTransition; }
            set { _normalTransition = value; }
        }

        public override Transition SelectTransition(object from, object to, SceReaderNavigationMode navigationMode, DependencyObject container)
        {
            Transition transition = null;
            foreach (SelectorDelegate dlgt in Selectors)
            {
                if (dlgt != null)
                {
                    transition = dlgt(from, to, navigationMode);
                    if (transition != null)
                    {
                        break;
                    }
                }
            }

            return transition;
        }

        private Transition TranslateTransitionFromPrototype(Transition prototype, TranslateDirection direction)
        {
            if (prototype != null)
            {
                TranslateTransition translatePrototype = prototype as TranslateTransition;
                if (translatePrototype != null)
                {
                    translatePrototype.SetDirection(direction);
                    return translatePrototype;
                }
            }

            return prototype;
        }

        private Transition TransitionNext(object from, object to, SceReaderNavigationMode mode)
        {
            if (mode == SceReaderNavigationMode.Next || 
                mode == SceReaderNavigationMode.Forward)
            {
                if (from is SectionNavigator && to is SectionNavigator)
                {
                    return TranslateTransitionFromPrototype(NextTransition, TranslateDirection.TranslateUp);
                }
                else
                {
                    return TranslateTransitionFromPrototype(NextTransition, TranslateDirection.TranslateLeft);
                }
            }
            return null;
        }

        private Transition TransitionPrevious(object from, object to, SceReaderNavigationMode mode)
        {
            if (mode == SceReaderNavigationMode.Previous ||
                mode == SceReaderNavigationMode.Back)
            {
                if (from is SectionNavigator && to is SectionNavigator)
                {
                    return TranslateTransitionFromPrototype(PreviousTransition, TranslateDirection.TranslateDown);
                }
                else
                {
                    return TranslateTransitionFromPrototype(PreviousTransition, TranslateDirection.TranslateRight);
                }
            }
            return null;
        }

        private Transition TransitionNormal(object from, object to, SceReaderNavigationMode mode)
        {
            if (mode == SceReaderNavigationMode.Normal)
            {
                return NormalTransition;
            }
            return null;
        }
    }
}