//***************************************************************************
//
//    Copyright (c) Microsoft Corporation. All rights reserved.
//    This code is licensed under the Visual Studio SDK license terms.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//***************************************************************************

﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) Microsoft Corporation.  All rights reserved.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Diagnostics.CodeAnalysis;

[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.VisualStudio.Language.Spellchecker", Justification = "Initialized via MEF")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.VisualStudio.Language.Spellchecker.CommentTextTagger+CommentTextTaggerProvider.#set_ClassifierAggregatorService(Microsoft.VisualStudio.Text.Classification.IClassifierAggregatorService)", Justification = "Initialized via MEF")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.VisualStudio.Language.Spellchecker.NaturalTextTagger.#.ctor(Microsoft.VisualStudio.Text.ITextBuffer)", Justification = "Initialized via MEF")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.VisualStudio.Language.Spellchecker.SpellSmartTagger+SpellSmartTaggerProvider.#set_SpellCheckingProvider(Microsoft.VisualStudio.Language.Spellchecker.ISpellcheckingProvider)", Justification = "Initialized via MEF")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.VisualStudio.Language.Spellchecker.SpellSmartTagger+SpellSmartTaggerProvider.#set_TagAggregatorFactory(Microsoft.VisualStudio.Text.Tagging.IBufferTagAggregatorFactoryService)", Justification = "Initialized via MEF")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.VisualStudio.Language.Spellchecker.SquiggleTagger+SquiggleTaggerProvider.#set_SpellCheckingProvider(Microsoft.VisualStudio.Language.Spellchecker.ISpellcheckingProvider)", Justification = "Initialized via MEF")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.VisualStudio.Language.Spellchecker.SquiggleTagger+SquiggleTaggerProvider.#set_TagAggregatorFactory(Microsoft.VisualStudio.Text.Tagging.IBufferTagAggregatorFactoryService)", Justification = "Initialized via MEF")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.VisualStudio.Language.Spellchecker.SpellingDictionaryService.#LoadIgnoreDictionary()", Justification = "Initialized via MEF")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.VisualStudio.Language.Spellchecker.SpellingDictionaryService+CachedSpan.#get_Suggestions()", Justification = "Initialized via MEF")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.VisualStudio.Language.Spellchecker.SpellingDictionaryService+SpanCache.#set_CleanupRate(System.UInt32)", Justification = "Initialized via MEF")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.VisualStudio.Language.Spellchecker.SpellingDictionaryService+SpanCache.#get_Item(System.String)", Justification = "Initialized via MEF")]
[module: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.VisualStudio.Language.Spellchecker.SpellingDictionaryService+SpanCache.#set_MaxCacheCount(System.UInt32)", Justification = "Initialized via MEF")]

[module: SuppressMessage("Microsoft.Performance","CA1812:AvoidUninstantiatedInternalClasses", Scope="type", Target="Microsoft.VisualStudio.Language.Spellchecker.NaturalTextTagger+NaturalTextTaggerProvider")]
[module: SuppressMessage("Microsoft.Performance","CA1812:AvoidUninstantiatedInternalClasses", Scope="type", Target="Microsoft.VisualStudio.Language.Spellchecker.SpellSmartTagger+SpellSmartTaggerProvider")]
[module: SuppressMessage("Microsoft.Performance","CA1812:AvoidUninstantiatedInternalClasses", Scope="type", Target="Microsoft.VisualStudio.Language.Spellchecker.SquiggleTagger+SquiggleTaggerProvider")]
