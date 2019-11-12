#pragma once

//
// Disable a set of /W4 warnings that are violated universally throughout this sample.
//      C4054: 'type cast' : from function pointer to data pointer
//      C4057: nonstandard extension used : TYPE1 differs in indirection to slightly different base types TYPE2
//      C4127: conditional expression is constant
//      C4201: nonstandard extension used : nameless struct/union
//      C4213: nonstandard extension used : cast on l-value
//      C4706: nonstandard extension used : assignment within conditional expression
//

#pragma warning(disable:4054)
#pragma warning(disable:4057)
#pragma warning(disable:4127)
#pragma warning(disable:4201)
#pragma warning(disable:4213)
#pragma warning(disable:4706)
