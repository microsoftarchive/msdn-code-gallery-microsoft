IntraTextAdornmentSample

This sample adds intra-text adornments to the editor.
Intra-text adornments are adornments that are shown between text characters, as opposed to behind or in front.
Intra-text adornments may optionally replace text.

In this particular sample, the intra-text adornments are color swatches that replace six digit hex numbers.
To have the color swatches appear next to the numbers and not instead of them,
comment out the "#define HIDING_TEXT" in ColorAdornmentTagger.cs.

Data and UI components are separated:
 - ColorTag, ColorTagger, and ColorTaggerProvider constitute the data only portion of the sample.
 - ColorAdornment, ColorAdornmentTagger, and ColorAdornmentTaggerProvider are the UI side.

The Support\ directory contains reusable helper base classes:
 - IntraTextAdornmentTagger :
		Takes care of the grunt work of providing intra-text adornments.
 - IntraTextAdornmentTagTransformer :
		Specialization of IntraTextAdornmentTagger for providing adornments based on data tags.
 - RegexTagger :
		Helps write simple regular expression based taggers.
