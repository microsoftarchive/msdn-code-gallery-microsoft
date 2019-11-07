/*
**  Microsoft Excel Developer's Toolkit
**  Version 12.0
**
**  File:           INCLUDE\XLCALL.H
**  Description:    Header file for for Excel callbacks
**  Platform:       Microsoft Windows
**
**  DEPENDENCY:
**  Include <windows.h> before you include this.
**
**  This file defines the constants and
**  data types which are used in the
**  Microsoft Excel C API.
**
*/

#pragma once

/*
** XL 12 Basic Datatypes 
**/

typedef INT32 BOOL;			/* Boolean */
typedef WCHAR XCHAR;			/* Wide Character */
typedef INT32 RW;			/* XL 12 Row */
typedef INT32 COL;	 	      	/* XL 12 Column */
typedef DWORD_PTR IDSHEET;		/* XL12 Sheet ID */

/*
** XLREF structure 
**
** Describes a single rectangular reference.
*/

typedef struct xlref 
{
	WORD rwFirst;
	WORD rwLast;
	BYTE colFirst;
	BYTE colLast;
} XLREF, *LPXLREF;


/*
** XLMREF structure
**
** Describes multiple rectangular references.
** This is a variable size structure, default 
** size is 1 reference.
*/

typedef struct xlmref 
{
	WORD count;
	XLREF reftbl[1];					/* actually reftbl[count] */
} XLMREF, *LPXLMREF;


/*
** XLREF12 structure 
**
** Describes a single XL 12 rectangular reference.
*/

typedef struct xlref12
{
	RW rwFirst;
	RW rwLast;
	COL colFirst;
	COL colLast;
} XLREF12, *LPXLREF12;


/*
** XLMREF12 structure
**
** Describes multiple rectangular XL 12 references.
** This is a variable size structure, default 
** size is 1 reference.
*/

typedef struct xlmref12
{
	WORD count;
	XLREF12 reftbl[1];					/* actually reftbl[count] */
} XLMREF12, *LPXLMREF12;


/*
** FP structure
**
** Describes FP structure.
*/

typedef struct _FP
{
    unsigned short int rows;
    unsigned short int columns;
    double array[1];        /* Actually, array[rows][columns] */
} FP;

/*
** FP12 structure
**
** Describes FP structure capable of handling the big grid.
*/

typedef struct _FP12
{
    INT32 rows;
    INT32 columns;
    double array[1];        /* Actually, array[rows][columns] */
} FP12;


/*
** XLOPER structure 
**
** Excel's fundamental data type: can hold data
** of any type. Use "R" as the argument type in the 
** REGISTER function.
**/

typedef struct xloper 
{
	union 
	{
		double num;					/* xltypeNum */
		LPSTR str;					/* xltypeStr */
#ifdef __cplusplus
		WORD xbool;					/* xltypeBool */
#else	
		WORD bool;					/* xltypeBool */
#endif	
		WORD err;					/* xltypeErr */
		short int w;					/* xltypeInt */
		struct 
		{
			WORD count;				/* always = 1 */
			XLREF ref;
		} sref;						/* xltypeSRef */
		struct 
		{
			XLMREF *lpmref;
			IDSHEET idSheet;
		} mref;						/* xltypeRef */
		struct 
		{
			struct xloper *lparray;
			WORD rows;
			WORD columns;
		} array;					/* xltypeMulti */
		struct 
		{
			union
			{
				short int level;		/* xlflowRestart */
				short int tbctrl;		/* xlflowPause */
				IDSHEET idSheet;		/* xlflowGoto */
			} valflow;
			WORD rw;				/* xlflowGoto */
			BYTE col;				/* xlflowGoto */
			BYTE xlflow;
		} flow;						/* xltypeFlow */
		struct
		{
			union
			{
				BYTE *lpbData;			/* data passed to XL */
				HANDLE hdata;			/* data returned from XL */
			} h;
			long cbData;
		} bigdata;					/* xltypeBigData */
	} val;
	WORD xltype;
} XLOPER, *LPXLOPER;

/*
** XLOPER12 structure 
**
** Excel 12's fundamental data type: can hold data
** of any type. Use "U" as the argument type in the 
** REGISTER function.
**/

typedef struct xloper12 
{
	union 
	{
		double num;				       	/* xltypeNum */
		XCHAR *str;				       	/* xltypeStr */
		BOOL xbool;				       	/* xltypeBool */
		int err;				       	/* xltypeErr */
		int w;
		struct 
		{
			WORD count;			       	/* always = 1 */
			XLREF12 ref;
		} sref;						/* xltypeSRef */
		struct 
		{
			XLMREF12 *lpmref;
			IDSHEET idSheet;
		} mref;						/* xltypeRef */
		struct 
		{
			struct xloper12 *lparray;
			RW rows;
			COL columns;
		} array;					/* xltypeMulti */
		struct 
		{
			union
			{
				int level;			/* xlflowRestart */
				int tbctrl;			/* xlflowPause */
				IDSHEET idSheet;		/* xlflowGoto */
			} valflow;
			RW rw;				       	/* xlflowGoto */
			COL col;			       	/* xlflowGoto */
			BYTE xlflow;
		} flow;						/* xltypeFlow */
		struct
		{
			union
			{
				BYTE *lpbData;			/* data passed to XL */
				HANDLE hdata;			/* data returned from XL */
			} h;
			long cbData;
		} bigdata;					/* xltypeBigData */
	} val;
	DWORD xltype;
} XLOPER12, *LPXLOPER12;

/*
** XLOPER and XLOPER12 data types
**
** Used for xltype field of XLOPER and XLOPER12 structures
*/

#define xltypeNum        0x0001
#define xltypeStr        0x0002
#define xltypeBool       0x0004
#define xltypeRef        0x0008
#define xltypeErr        0x0010
#define xltypeFlow       0x0020
#define xltypeMulti      0x0040
#define xltypeMissing    0x0080
#define xltypeNil        0x0100
#define xltypeSRef       0x0400
#define xltypeInt        0x0800

#define xlbitXLFree      0x1000
#define xlbitDLLFree     0x4000

#define xltypeBigData	(xltypeStr | xltypeInt)


/*
** Error codes
**
** Used for val.err field of XLOPER and XLOPER12 structures
** when constructing error XLOPERs and XLOPER12s
*/

#define xlerrNull    0
#define xlerrDiv0    7
#define xlerrValue   15
#define xlerrRef     23
#define xlerrName    29
#define xlerrNum     36
#define xlerrNA      42
#define xlerrGettingData 43


/* 
** Flow data types
**
** Used for val.flow.xlflow field of XLOPER and XLOPER12 structures
** when constructing flow-control XLOPERs and XLOPER12s
**/

#define xlflowHalt       1
#define xlflowGoto       2
#define xlflowRestart    8
#define xlflowPause      16
#define xlflowResume     64


/*
** Return codes
**
** These values can be returned from Excel4(), Excel4v(), Excel12() or Excel12v().
*/

#define xlretSuccess        0    /* success */ 
#define xlretAbort          1    /* macro halted */
#define xlretInvXlfn        2    /* invalid function number */ 
#define xlretInvCount       4    /* invalid number of arguments */ 
#define xlretInvXloper      8    /* invalid OPER structure */  
#define xlretStackOvfl      16   /* stack overflow */  
#define xlretFailed         32   /* command failed */  
#define xlretUncalced       64   /* uncalced cell */
#define xlretNotThreadSafe  128  /* not allowed during multi-threaded calc */
#define xlretInvAsynchronousContext  256  /* invalid asynchronous function handle */
#define xlretNotClusterSafe  512  /* not supported on cluster */


/*
** XLL events
**
** Passed in to an xlEventRegister call to register a corresponding event.
*/

#define xleventCalculationEnded      1    /* Fires at the end of calculation */ 
#define xleventCalculationCanceled   2    /* Fires when calculation is interrupted */


/*
** Function prototypes
*/

#ifdef __cplusplus
extern "C" {
#endif

int _cdecl Excel4(int xlfn, LPXLOPER operRes, int count,... ); 
/* followed by count LPXLOPERs */

int pascal Excel4v(int xlfn, LPXLOPER operRes, int count, LPXLOPER opers[]);

int pascal XLCallVer(void);

long pascal LPenHelper(int wCode, VOID *lpv);

int _cdecl Excel12(int xlfn, LPXLOPER12 operRes, int count,... );
/* followed by count LPXLOPER12s */

int pascal Excel12v(int xlfn, LPXLOPER12 operRes, int count, LPXLOPER12 opers[]);

#ifdef __cplusplus
}
#endif


/*
** Cluster Connector Async Callback
*/

typedef int (CALLBACK *PXL_HPC_ASYNC_CALLBACK)(DWORD dwAsyncHandle, LPXLOPER12 operReturn);


/*
** Function number bits
*/ 

#define xlCommand    0x8000
#define xlSpecial    0x4000
#define xlIntl       0x2000
#define xlPrompt     0x1000


/*
** Auxiliary function numbers
**
** These functions are available only from the C API,
** not from the Excel macro language.
*/

#define xlFree          (0  | xlSpecial)
#define xlStack         (1  | xlSpecial)
#define xlCoerce        (2  | xlSpecial)
#define xlSet           (3  | xlSpecial)
#define xlSheetId       (4  | xlSpecial)
#define xlSheetNm       (5  | xlSpecial)
#define xlAbort         (6  | xlSpecial)
#define xlGetInst       (7  | xlSpecial)
#define xlGetHwnd       (8  | xlSpecial)
#define xlGetName       (9  | xlSpecial)
#define xlEnableXLMsgs  (10 | xlSpecial)
#define xlDisableXLMsgs (11 | xlSpecial)
#define xlDefineBinaryName (12 | xlSpecial)
#define xlGetBinaryName	(13 | xlSpecial)
/* GetFooInfo are valid only for calls to LPenHelper */
#define xlGetFmlaInfo	(14 | xlSpecial)
#define xlGetMouseInfo	(15 | xlSpecial)
#define xlAsyncReturn	(16 | xlSpecial)	/*Set return value from an asynchronous function call*/
#define xlEventRegister	(17 | xlSpecial)	/*Register an XLL event*/
#define xlRunningOnCluster	(18 | xlSpecial)	/*Returns true if running on Compute Cluster*/

/* edit modes */
#define xlModeReady	0	// not in edit mode
#define xlModeEnter	1	// enter mode
#define xlModeEdit	2	// edit mode
#define xlModePoint	4	// point mode

/* document(page) types */
#define dtNil 0x7f	// window is not a sheet, macro, chart or basic
// OR window is not the selected window at idle state
#define dtSheet 0	// sheet
#define dtProc  1	// XLM macro
#define dtChart 2	// Chart
#define dtBasic 6	// VBA 

/* hit test codes */
#define htNone		0x00	// none of below
#define htClient	0x01	// internal for "in the client are", should never see
#define htVSplit	0x02	// vertical split area with split panes
#define htHSplit	0x03	// horizontal split area
#define htColWidth	0x04	// column width adjuster area
#define htRwHeight	0x05	// row height adjuster area
#define htRwColHdr	0x06	// the intersection of row and column headers
#define htObject	0x07	// the body of an object
// the following are for size handles of draw objects
#define htTopLeft	0x08
#define htBotLeft	0x09
#define htLeft		0x0A
#define htTopRight	0x0B
#define htBotRight	0x0C
#define htRight		0x0D
#define htTop		0x0E
#define htBot		0x0F
// end size handles
#define htRwGut		0x10	// row area of outline gutter
#define htColGut	0x11	// column area of outline gutter
#define htTextBox	0x12	// body of a text box (where we shouw I-Beam cursor)
#define htRwLevels	0x13	// row levels buttons of outline gutter
#define htColLevels	0x14	// column levels buttons of outline gutter
#define htDman		0x15	// the drag/drop handle of the selection
#define htDmanFill	0x16	// the auto-fill handle of the selection
#define htXSplit	0x17	// the intersection of the horz & vert pane splits
#define htVertex	0x18	// a vertex of a polygon draw object
#define htAddVtx	0x19	// htVertex in add a vertex mode
#define htDelVtx	0x1A	// htVertex in delete a vertex mode
#define htRwHdr		0x1B	// row header
#define htColHdr	0x1C	// column header
#define htRwShow	0x1D	// Like htRowHeight except means grow a hidden column
#define htColShow	0x1E	// column version of htRwShow
#define htSizing	0x1F	// Internal use only
#define htSxpivot	0x20	// a drag/drop tile in a pivot table
#define htTabs		0x21	// the sheet paging tabs
#define htEdit		0x22	// Internal use only

typedef struct _fmlainfo
{
	int wPointMode;	// current edit mode.  0 => rest of struct undefined
	int cch;	// count of characters in formula
	char *lpch;	// pointer to formula characters.  READ ONLY!!!
	int ichFirst;	// char offset to start of selection
	int ichLast;	// char offset to end of selection (may be > cch)
	int ichCaret;	// char offset to blinking caret
} FMLAINFO;

typedef struct _mouseinfo
{
	/* input section */
	HWND hwnd;		// window to get info on
	POINT pt;		// mouse position to get info on

	/* output section */
	int dt;			// document(page) type
	int ht;			// hit test code
	int rw;			// row @ mouse (-1 if #n/a)
	int col;		// col @ mouse (-1 if #n/a)
} MOUSEINFO;



/* 
** User defined function
**
** First argument should be a function reference.
*/

#define xlUDF      255


/*
** Built-in Excel functions and command equivalents
*/


// Excel function numbers

#define xlfCount 0
#define xlfIsna 2
#define xlfIserror 3
#define xlfSum 4
#define xlfAverage 5
#define xlfMin 6
#define xlfMax 7
#define xlfRow 8
#define xlfColumn 9
#define xlfNa 10
#define xlfNpv 11
#define xlfStdev 12
#define xlfDollar 13
#define xlfFixed 14
#define xlfSin 15
#define xlfCos 16
#define xlfTan 17
#define xlfAtan 18
#define xlfPi 19
#define xlfSqrt 20
#define xlfExp 21
#define xlfLn 22
#define xlfLog10 23
#define xlfAbs 24
#define xlfInt 25
#define xlfSign 26
#define xlfRound 27
#define xlfLookup 28
#define xlfIndex 29
#define xlfRept 30
#define xlfMid 31
#define xlfLen 32
#define xlfValue 33
#define xlfTrue 34
#define xlfFalse 35
#define xlfAnd 36
#define xlfOr 37
#define xlfNot 38
#define xlfMod 39
#define xlfDcount 40
#define xlfDsum 41
#define xlfDaverage 42
#define xlfDmin 43
#define xlfDmax 44
#define xlfDstdev 45
#define xlfVar 46
#define xlfDvar 47
#define xlfText 48
#define xlfLinest 49
#define xlfTrend 50
#define xlfLogest 51
#define xlfGrowth 52
#define xlfGoto 53
#define xlfHalt 54
#define xlfPv 56
#define xlfFv 57
#define xlfNper 58
#define xlfPmt 59
#define xlfRate 60
#define xlfMirr 61
#define xlfIrr 62
#define xlfRand 63
#define xlfMatch 64
#define xlfDate 65
#define xlfTime 66
#define xlfDay 67
#define xlfMonth 68
#define xlfYear 69
#define xlfWeekday 70
#define xlfHour 71
#define xlfMinute 72
#define xlfSecond 73
#define xlfNow 74
#define xlfAreas 75
#define xlfRows 76
#define xlfColumns 77
#define xlfOffset 78
#define xlfAbsref 79
#define xlfRelref 80
#define xlfArgument 81
#define xlfSearch 82
#define xlfTranspose 83
#define xlfError 84
#define xlfStep 85
#define xlfType 86
#define xlfEcho 87
#define xlfSetName 88
#define xlfCaller 89
#define xlfDeref 90
#define xlfWindows 91
#define xlfSeries 92
#define xlfDocuments 93
#define xlfActiveCell 94
#define xlfSelection 95
#define xlfResult 96
#define xlfAtan2 97
#define xlfAsin 98
#define xlfAcos 99
#define xlfChoose 100
#define xlfHlookup 101
#define xlfVlookup 102
#define xlfLinks 103
#define xlfInput 104
#define xlfIsref 105
#define xlfGetFormula 106
#define xlfGetName 107
#define xlfSetValue 108
#define xlfLog 109
#define xlfExec 110
#define xlfChar 111
#define xlfLower 112
#define xlfUpper 113
#define xlfProper 114
#define xlfLeft 115
#define xlfRight 116
#define xlfExact 117
#define xlfTrim 118
#define xlfReplace 119
#define xlfSubstitute 120
#define xlfCode 121
#define xlfNames 122
#define xlfDirectory 123
#define xlfFind 124
#define xlfCell 125
#define xlfIserr 126
#define xlfIstext 127
#define xlfIsnumber 128
#define xlfIsblank 129
#define xlfT 130
#define xlfN 131
#define xlfFopen 132
#define xlfFclose 133
#define xlfFsize 134
#define xlfFreadln 135
#define xlfFread 136
#define xlfFwriteln 137
#define xlfFwrite 138
#define xlfFpos 139
#define xlfDatevalue 140
#define xlfTimevalue 141
#define xlfSln 142
#define xlfSyd 143
#define xlfDdb 144
#define xlfGetDef 145
#define xlfReftext 146
#define xlfTextref 147
#define xlfIndirect 148
#define xlfRegister 149
#define xlfCall 150
#define xlfAddBar 151
#define xlfAddMenu 152
#define xlfAddCommand 153
#define xlfEnableCommand 154
#define xlfCheckCommand 155
#define xlfRenameCommand 156
#define xlfShowBar 157
#define xlfDeleteMenu 158
#define xlfDeleteCommand 159
#define xlfGetChartItem 160
#define xlfDialogBox 161
#define xlfClean 162
#define xlfMdeterm 163
#define xlfMinverse 164
#define xlfMmult 165
#define xlfFiles 166
#define xlfIpmt 167
#define xlfPpmt 168
#define xlfCounta 169
#define xlfCancelKey 170
#define xlfInitiate 175
#define xlfRequest 176
#define xlfPoke 177
#define xlfExecute 178
#define xlfTerminate 179
#define xlfRestart 180
#define xlfHelp 181
#define xlfGetBar 182
#define xlfProduct 183
#define xlfFact 184
#define xlfGetCell 185
#define xlfGetWorkspace 186
#define xlfGetWindow 187
#define xlfGetDocument 188
#define xlfDproduct 189
#define xlfIsnontext 190
#define xlfGetNote 191
#define xlfNote 192
#define xlfStdevp 193
#define xlfVarp 194
#define xlfDstdevp 195
#define xlfDvarp 196
#define xlfTrunc 197
#define xlfIslogical 198
#define xlfDcounta 199
#define xlfDeleteBar 200
#define xlfUnregister 201
#define xlfUsdollar 204
#define xlfFindb 205
#define xlfSearchb 206
#define xlfReplaceb 207
#define xlfLeftb 208
#define xlfRightb 209
#define xlfMidb 210
#define xlfLenb 211
#define xlfRoundup 212
#define xlfRounddown 213
#define xlfAsc 214
#define xlfDbcs 215
#define xlfRank 216
#define xlfAddress 219
#define xlfDays360 220
#define xlfToday 221
#define xlfVdb 222
#define xlfMedian 227
#define xlfSumproduct 228
#define xlfSinh 229
#define xlfCosh 230
#define xlfTanh 231
#define xlfAsinh 232
#define xlfAcosh 233
#define xlfAtanh 234
#define xlfDget 235
#define xlfCreateObject 236
#define xlfVolatile 237
#define xlfLastError 238
#define xlfCustomUndo 239
#define xlfCustomRepeat 240
#define xlfFormulaConvert 241
#define xlfGetLinkInfo 242
#define xlfTextBox 243
#define xlfInfo 244
#define xlfGroup 245
#define xlfGetObject 246
#define xlfDb 247
#define xlfPause 248
#define xlfResume 251
#define xlfFrequency 252
#define xlfAddToolbar 253
#define xlfDeleteToolbar 254
#define xlfResetToolbar 256
#define xlfEvaluate 257
#define xlfGetToolbar 258
#define xlfGetTool 259
#define xlfSpellingCheck 260
#define xlfErrorType 261
#define xlfAppTitle 262
#define xlfWindowTitle 263
#define xlfSaveToolbar 264
#define xlfEnableTool 265
#define xlfPressTool 266
#define xlfRegisterId 267
#define xlfGetWorkbook 268
#define xlfAvedev 269
#define xlfBetadist 270
#define xlfGammaln 271
#define xlfBetainv 272
#define xlfBinomdist 273
#define xlfChidist 274
#define xlfChiinv 275
#define xlfCombin 276
#define xlfConfidence 277
#define xlfCritbinom 278
#define xlfEven 279
#define xlfExpondist 280
#define xlfFdist 281
#define xlfFinv 282
#define xlfFisher 283
#define xlfFisherinv 284
#define xlfFloor 285
#define xlfGammadist 286
#define xlfGammainv 287
#define xlfCeiling 288
#define xlfHypgeomdist 289
#define xlfLognormdist 290
#define xlfLoginv 291
#define xlfNegbinomdist 292
#define xlfNormdist 293
#define xlfNormsdist 294
#define xlfNorminv 295
#define xlfNormsinv 296
#define xlfStandardize 297
#define xlfOdd 298
#define xlfPermut 299
#define xlfPoisson 300
#define xlfTdist 301
#define xlfWeibull 302
#define xlfSumxmy2 303
#define xlfSumx2my2 304
#define xlfSumx2py2 305
#define xlfChitest 306
#define xlfCorrel 307
#define xlfCovar 308
#define xlfForecast 309
#define xlfFtest 310
#define xlfIntercept 311
#define xlfPearson 312
#define xlfRsq 313
#define xlfSteyx 314
#define xlfSlope 315
#define xlfTtest 316
#define xlfProb 317
#define xlfDevsq 318
#define xlfGeomean 319
#define xlfHarmean 320
#define xlfSumsq 321
#define xlfKurt 322
#define xlfSkew 323
#define xlfZtest 324
#define xlfLarge 325
#define xlfSmall 326
#define xlfQuartile 327
#define xlfPercentile 328
#define xlfPercentrank 329
#define xlfMode 330
#define xlfTrimmean 331
#define xlfTinv 332
#define xlfMovieCommand 334
#define xlfGetMovie 335
#define xlfConcatenate 336
#define xlfPower 337
#define xlfPivotAddData 338
#define xlfGetPivotTable 339
#define xlfGetPivotField 340
#define xlfGetPivotItem 341
#define xlfRadians 342
#define xlfDegrees 343
#define xlfSubtotal 344
#define xlfSumif 345
#define xlfCountif 346
#define xlfCountblank 347
#define xlfScenarioGet 348
#define xlfOptionsListsGet 349
#define xlfIspmt 350
#define xlfDatedif 351
#define xlfDatestring 352
#define xlfNumberstring 353
#define xlfRoman 354
#define xlfOpenDialog 355
#define xlfSaveDialog 356
#define xlfViewGet 357
#define xlfGetpivotdata 358
#define xlfHyperlink 359
#define xlfPhonetic 360
#define xlfAveragea 361
#define xlfMaxa 362
#define xlfMina 363
#define xlfStdevpa 364
#define xlfVarpa 365
#define xlfStdeva 366
#define xlfVara 367
#define xlfBahttext 368
#define xlfThaidayofweek 369
#define xlfThaidigit 370
#define xlfThaimonthofyear 371
#define xlfThainumsound 372
#define xlfThainumstring 373
#define xlfThaistringlength 374
#define xlfIsthaidigit 375
#define xlfRoundbahtdown 376
#define xlfRoundbahtup 377
#define xlfThaiyear 378
#define xlfRtd 379
#define xlfCubevalue 380
#define xlfCubemember 381
#define xlfCubememberproperty 382
#define xlfCuberankedmember 383
#define xlfHex2bin 384
#define xlfHex2dec 385
#define xlfHex2oct 386
#define xlfDec2bin 387
#define xlfDec2hex 388
#define xlfDec2oct 389
#define xlfOct2bin 390
#define xlfOct2hex 391
#define xlfOct2dec 392
#define xlfBin2dec 393
#define xlfBin2oct 394
#define xlfBin2hex 395
#define xlfImsub 396
#define xlfImdiv 397
#define xlfImpower 398
#define xlfImabs 399
#define xlfImsqrt 400
#define xlfImln 401
#define xlfImlog2 402
#define xlfImlog10 403
#define xlfImsin 404
#define xlfImcos 405
#define xlfImexp 406
#define xlfImargument 407
#define xlfImconjugate 408
#define xlfImaginary 409
#define xlfImreal 410
#define xlfComplex 411
#define xlfImsum 412
#define xlfImproduct 413
#define xlfSeriessum 414
#define xlfFactdouble 415
#define xlfSqrtpi 416
#define xlfQuotient 417
#define xlfDelta 418
#define xlfGestep 419
#define xlfIseven 420
#define xlfIsodd 421
#define xlfMround 422
#define xlfErf 423
#define xlfErfc 424
#define xlfBesselj 425
#define xlfBesselk 426
#define xlfBessely 427
#define xlfBesseli 428
#define xlfXirr 429
#define xlfXnpv 430
#define xlfPricemat 431
#define xlfYieldmat 432
#define xlfIntrate 433
#define xlfReceived 434
#define xlfDisc 435
#define xlfPricedisc 436
#define xlfYielddisc 437
#define xlfTbilleq 438
#define xlfTbillprice 439
#define xlfTbillyield 440
#define xlfPrice 441
#define xlfYield 442
#define xlfDollarde 443
#define xlfDollarfr 444
#define xlfNominal 445
#define xlfEffect 446
#define xlfCumprinc 447
#define xlfCumipmt 448
#define xlfEdate 449
#define xlfEomonth 450
#define xlfYearfrac 451
#define xlfCoupdaybs 452
#define xlfCoupdays 453
#define xlfCoupdaysnc 454
#define xlfCoupncd 455
#define xlfCoupnum 456
#define xlfCouppcd 457
#define xlfDuration 458
#define xlfMduration 459
#define xlfOddlprice 460
#define xlfOddlyield 461
#define xlfOddfprice 462
#define xlfOddfyield 463
#define xlfRandbetween 464
#define xlfWeeknum 465
#define xlfAmordegrc 466
#define xlfAmorlinc 467
#define xlfConvert 468
#define xlfAccrint 469
#define xlfAccrintm 470
#define xlfWorkday 471
#define xlfNetworkdays 472
#define xlfGcd 473
#define xlfMultinomial 474
#define xlfLcm 475
#define xlfFvschedule 476
#define xlfCubekpimember 477
#define xlfCubeset 478
#define xlfCubesetcount 479
#define xlfIferror 480
#define xlfCountifs 481
#define xlfSumifs 482
#define xlfAverageif 483
#define xlfAverageifs 484
#define xlfAggregate 485
#define xlfBinom_dist 486
#define xlfBinom_inv 487
#define xlfConfidence_norm 488
#define xlfConfidence_t 489
#define xlfChisq_test 490
#define xlfF_test 491
#define xlfCovariance_p 492
#define xlfCovariance_s 493
#define xlfExpon_dist 494
#define xlfGamma_dist 495
#define xlfGamma_inv 496
#define xlfMode_mult 497
#define xlfMode_sngl 498
#define xlfNorm_dist 499
#define xlfNorm_inv 500
#define xlfPercentile_exc 501
#define xlfPercentile_inc 502
#define xlfPercentrank_exc 503
#define xlfPercentrank_inc 504
#define xlfPoisson_dist 505
#define xlfQuartile_exc 506
#define xlfQuartile_inc 507
#define xlfRank_avg 508
#define xlfRank_eq 509
#define xlfStdev_s 510
#define xlfStdev_p 511
#define xlfT_dist 512
#define xlfT_dist_2t 513
#define xlfT_dist_rt 514
#define xlfT_inv 515
#define xlfT_inv_2t 516
#define xlfVar_s 517
#define xlfVar_p 518
#define xlfWeibull_dist 519
#define xlfNetworkdays_intl 520
#define xlfWorkday_intl 521
#define xlfEcma_ceiling 522
#define xlfIso_ceiling 523
#define xlfBeta_dist 525
#define xlfBeta_inv 526
#define xlfChisq_dist 527
#define xlfChisq_dist_rt 528
#define xlfChisq_inv 529
#define xlfChisq_inv_rt 530
#define xlfF_dist 531
#define xlfF_dist_rt 532
#define xlfF_inv 533
#define xlfF_inv_rt 534
#define xlfHypgeom_dist 535
#define xlfLognorm_dist 536
#define xlfLognorm_inv 537
#define xlfNegbinom_dist 538
#define xlfNorm_s_dist 539
#define xlfNorm_s_inv 540
#define xlfT_test 541
#define xlfZ_test 542

/* Excel command numbers */
#define xlcBeep (0 | xlCommand)
#define xlcOpen (1 | xlCommand)
#define xlcOpenLinks (2 | xlCommand)
#define xlcCloseAll (3 | xlCommand)
#define xlcSave (4 | xlCommand)
#define xlcSaveAs (5 | xlCommand)
#define xlcFileDelete (6 | xlCommand)
#define xlcPageSetup (7 | xlCommand)
#define xlcPrint (8 | xlCommand)
#define xlcPrinterSetup (9 | xlCommand)
#define xlcQuit (10 | xlCommand)
#define xlcNewWindow (11 | xlCommand)
#define xlcArrangeAll (12 | xlCommand)
#define xlcWindowSize (13 | xlCommand)
#define xlcWindowMove (14 | xlCommand)
#define xlcFull (15 | xlCommand)
#define xlcClose (16 | xlCommand)
#define xlcRun (17 | xlCommand)
#define xlcSetPrintArea (22 | xlCommand)
#define xlcSetPrintTitles (23 | xlCommand)
#define xlcSetPageBreak (24 | xlCommand)
#define xlcRemovePageBreak (25 | xlCommand)
#define xlcFont (26 | xlCommand)
#define xlcDisplay (27 | xlCommand)
#define xlcProtectDocument (28 | xlCommand)
#define xlcPrecision (29 | xlCommand)
#define xlcA1R1c1 (30 | xlCommand)
#define xlcCalculateNow (31 | xlCommand)
#define xlcCalculation (32 | xlCommand)
#define xlcDataFind (34 | xlCommand)
#define xlcExtract (35 | xlCommand)
#define xlcDataDelete (36 | xlCommand)
#define xlcSetDatabase (37 | xlCommand)
#define xlcSetCriteria (38 | xlCommand)
#define xlcSort (39 | xlCommand)
#define xlcDataSeries (40 | xlCommand)
#define xlcTable (41 | xlCommand)
#define xlcFormatNumber (42 | xlCommand)
#define xlcAlignment (43 | xlCommand)
#define xlcStyle (44 | xlCommand)
#define xlcBorder (45 | xlCommand)
#define xlcCellProtection (46 | xlCommand)
#define xlcColumnWidth (47 | xlCommand)
#define xlcUndo (48 | xlCommand)
#define xlcCut (49 | xlCommand)
#define xlcCopy (50 | xlCommand)
#define xlcPaste (51 | xlCommand)
#define xlcClear (52 | xlCommand)
#define xlcPasteSpecial (53 | xlCommand)
#define xlcEditDelete (54 | xlCommand)
#define xlcInsert (55 | xlCommand)
#define xlcFillRight (56 | xlCommand)
#define xlcFillDown (57 | xlCommand)
#define xlcDefineName (61 | xlCommand)
#define xlcCreateNames (62 | xlCommand)
#define xlcFormulaGoto (63 | xlCommand)
#define xlcFormulaFind (64 | xlCommand)
#define xlcSelectLastCell (65 | xlCommand)
#define xlcShowActiveCell (66 | xlCommand)
#define xlcGalleryArea (67 | xlCommand)
#define xlcGalleryBar (68 | xlCommand)
#define xlcGalleryColumn (69 | xlCommand)
#define xlcGalleryLine (70 | xlCommand)
#define xlcGalleryPie (71 | xlCommand)
#define xlcGalleryScatter (72 | xlCommand)
#define xlcCombination (73 | xlCommand)
#define xlcPreferred (74 | xlCommand)
#define xlcAddOverlay (75 | xlCommand)
#define xlcGridlines (76 | xlCommand)
#define xlcSetPreferred (77 | xlCommand)
#define xlcAxes (78 | xlCommand)
#define xlcLegend (79 | xlCommand)
#define xlcAttachText (80 | xlCommand)
#define xlcAddArrow (81 | xlCommand)
#define xlcSelectChart (82 | xlCommand)
#define xlcSelectPlotArea (83 | xlCommand)
#define xlcPatterns (84 | xlCommand)
#define xlcMainChart (85 | xlCommand)
#define xlcOverlay (86 | xlCommand)
#define xlcScale (87 | xlCommand)
#define xlcFormatLegend (88 | xlCommand)
#define xlcFormatText (89 | xlCommand)
#define xlcEditRepeat (90 | xlCommand)
#define xlcParse (91 | xlCommand)
#define xlcJustify (92 | xlCommand)
#define xlcHide (93 | xlCommand)
#define xlcUnhide (94 | xlCommand)
#define xlcWorkspace (95 | xlCommand)
#define xlcFormula (96 | xlCommand)
#define xlcFormulaFill (97 | xlCommand)
#define xlcFormulaArray (98 | xlCommand)
#define xlcDataFindNext (99 | xlCommand)
#define xlcDataFindPrev (100 | xlCommand)
#define xlcFormulaFindNext (101 | xlCommand)
#define xlcFormulaFindPrev (102 | xlCommand)
#define xlcActivate (103 | xlCommand)
#define xlcActivateNext (104 | xlCommand)
#define xlcActivatePrev (105 | xlCommand)
#define xlcUnlockedNext (106 | xlCommand)
#define xlcUnlockedPrev (107 | xlCommand)
#define xlcCopyPicture (108 | xlCommand)
#define xlcSelect (109 | xlCommand)
#define xlcDeleteName (110 | xlCommand)
#define xlcDeleteFormat (111 | xlCommand)
#define xlcVline (112 | xlCommand)
#define xlcHline (113 | xlCommand)
#define xlcVpage (114 | xlCommand)
#define xlcHpage (115 | xlCommand)
#define xlcVscroll (116 | xlCommand)
#define xlcHscroll (117 | xlCommand)
#define xlcAlert (118 | xlCommand)
#define xlcNew (119 | xlCommand)
#define xlcCancelCopy (120 | xlCommand)
#define xlcShowClipboard (121 | xlCommand)
#define xlcMessage (122 | xlCommand)
#define xlcPasteLink (124 | xlCommand)
#define xlcAppActivate (125 | xlCommand)
#define xlcDeleteArrow (126 | xlCommand)
#define xlcRowHeight (127 | xlCommand)
#define xlcFormatMove (128 | xlCommand)
#define xlcFormatSize (129 | xlCommand)
#define xlcFormulaReplace (130 | xlCommand)
#define xlcSendKeys (131 | xlCommand)
#define xlcSelectSpecial (132 | xlCommand)
#define xlcApplyNames (133 | xlCommand)
#define xlcReplaceFont (134 | xlCommand)
#define xlcFreezePanes (135 | xlCommand)
#define xlcShowInfo (136 | xlCommand)
#define xlcSplit (137 | xlCommand)
#define xlcOnWindow (138 | xlCommand)
#define xlcOnData (139 | xlCommand)
#define xlcDisableInput (140 | xlCommand)
#define xlcEcho (141 | xlCommand)
#define xlcOutline (142 | xlCommand)
#define xlcListNames (143 | xlCommand)
#define xlcFileClose (144 | xlCommand)
#define xlcSaveWorkbook (145 | xlCommand)
#define xlcDataForm (146 | xlCommand)
#define xlcCopyChart (147 | xlCommand)
#define xlcOnTime (148 | xlCommand)
#define xlcWait (149 | xlCommand)
#define xlcFormatFont (150 | xlCommand)
#define xlcFillUp (151 | xlCommand)
#define xlcFillLeft (152 | xlCommand)
#define xlcDeleteOverlay (153 | xlCommand)
#define xlcNote (154 | xlCommand)
#define xlcShortMenus (155 | xlCommand)
#define xlcSetUpdateStatus (159 | xlCommand)
#define xlcColorPalette (161 | xlCommand)
#define xlcDeleteStyle (162 | xlCommand)
#define xlcWindowRestore (163 | xlCommand)
#define xlcWindowMaximize (164 | xlCommand)
#define xlcError (165 | xlCommand)
#define xlcChangeLink (166 | xlCommand)
#define xlcCalculateDocument (167 | xlCommand)
#define xlcOnKey (168 | xlCommand)
#define xlcAppRestore (169 | xlCommand)
#define xlcAppMove (170 | xlCommand)
#define xlcAppSize (171 | xlCommand)
#define xlcAppMinimize (172 | xlCommand)
#define xlcAppMaximize (173 | xlCommand)
#define xlcBringToFront (174 | xlCommand)
#define xlcSendToBack (175 | xlCommand)
#define xlcMainChartType (185 | xlCommand)
#define xlcOverlayChartType (186 | xlCommand)
#define xlcSelectEnd (187 | xlCommand)
#define xlcOpenMail (188 | xlCommand)
#define xlcSendMail (189 | xlCommand)
#define xlcStandardFont (190 | xlCommand)
#define xlcConsolidate (191 | xlCommand)
#define xlcSortSpecial (192 | xlCommand)
#define xlcGallery3dArea (193 | xlCommand)
#define xlcGallery3dColumn (194 | xlCommand)
#define xlcGallery3dLine (195 | xlCommand)
#define xlcGallery3dPie (196 | xlCommand)
#define xlcView3d (197 | xlCommand)
#define xlcGoalSeek (198 | xlCommand)
#define xlcWorkgroup (199 | xlCommand)
#define xlcFillGroup (200 | xlCommand)
#define xlcUpdateLink (201 | xlCommand)
#define xlcPromote (202 | xlCommand)
#define xlcDemote (203 | xlCommand)
#define xlcShowDetail (204 | xlCommand)
#define xlcUngroup (206 | xlCommand)
#define xlcObjectProperties (207 | xlCommand)
#define xlcSaveNewObject (208 | xlCommand)
#define xlcShare (209 | xlCommand)
#define xlcShareName (210 | xlCommand)
#define xlcDuplicate (211 | xlCommand)
#define xlcApplyStyle (212 | xlCommand)
#define xlcAssignToObject (213 | xlCommand)
#define xlcObjectProtection (214 | xlCommand)
#define xlcHideObject (215 | xlCommand)
#define xlcSetExtract (216 | xlCommand)
#define xlcCreatePublisher (217 | xlCommand)
#define xlcSubscribeTo (218 | xlCommand)
#define xlcAttributes (219 | xlCommand)
#define xlcShowToolbar (220 | xlCommand)
#define xlcPrintPreview (222 | xlCommand)
#define xlcEditColor (223 | xlCommand)
#define xlcShowLevels (224 | xlCommand)
#define xlcFormatMain (225 | xlCommand)
#define xlcFormatOverlay (226 | xlCommand)
#define xlcOnRecalc (227 | xlCommand)
#define xlcEditSeries (228 | xlCommand)
#define xlcDefineStyle (229 | xlCommand)
#define xlcLinePrint (240 | xlCommand)
#define xlcEnterData (243 | xlCommand)
#define xlcGalleryRadar (249 | xlCommand)
#define xlcMergeStyles (250 | xlCommand)
#define xlcEditionOptions (251 | xlCommand)
#define xlcPastePicture (252 | xlCommand)
#define xlcPastePictureLink (253 | xlCommand)
#define xlcSpelling (254 | xlCommand)
#define xlcZoom (256 | xlCommand)
#define xlcResume (258 | xlCommand)
#define xlcInsertObject (259 | xlCommand)
#define xlcWindowMinimize (260 | xlCommand)
#define xlcSize (261 | xlCommand)
#define xlcMove (262 | xlCommand)
#define xlcSoundNote (265 | xlCommand)
#define xlcSoundPlay (266 | xlCommand)
#define xlcFormatShape (267 | xlCommand)
#define xlcExtendPolygon (268 | xlCommand)
#define xlcFormatAuto (269 | xlCommand)
#define xlcGallery3dBar (272 | xlCommand)
#define xlcGallery3dSurface (273 | xlCommand)
#define xlcFillAuto (274 | xlCommand)
#define xlcCustomizeToolbar (276 | xlCommand)
#define xlcAddTool (277 | xlCommand)
#define xlcEditObject (278 | xlCommand)
#define xlcOnDoubleclick (279 | xlCommand)
#define xlcOnEntry (280 | xlCommand)
#define xlcWorkbookAdd (281 | xlCommand)
#define xlcWorkbookMove (282 | xlCommand)
#define xlcWorkbookCopy (283 | xlCommand)
#define xlcWorkbookOptions (284 | xlCommand)
#define xlcSaveWorkspace (285 | xlCommand)
#define xlcChartWizard (288 | xlCommand)
#define xlcDeleteTool (289 | xlCommand)
#define xlcMoveTool (290 | xlCommand)
#define xlcWorkbookSelect (291 | xlCommand)
#define xlcWorkbookActivate (292 | xlCommand)
#define xlcAssignToTool (293 | xlCommand)
#define xlcCopyTool (295 | xlCommand)
#define xlcResetTool (296 | xlCommand)
#define xlcConstrainNumeric (297 | xlCommand)
#define xlcPasteTool (298 | xlCommand)
#define xlcPlacement (300 | xlCommand)
#define xlcFillWorkgroup (301 | xlCommand)
#define xlcWorkbookNew (302 | xlCommand)
#define xlcScenarioCells (305 | xlCommand)
#define xlcScenarioDelete (306 | xlCommand)
#define xlcScenarioAdd (307 | xlCommand)
#define xlcScenarioEdit (308 | xlCommand)
#define xlcScenarioShow (309 | xlCommand)
#define xlcScenarioShowNext (310 | xlCommand)
#define xlcScenarioSummary (311 | xlCommand)
#define xlcPivotTableWizard (312 | xlCommand)
#define xlcPivotFieldProperties (313 | xlCommand)
#define xlcPivotField (314 | xlCommand)
#define xlcPivotItem (315 | xlCommand)
#define xlcPivotAddFields (316 | xlCommand)
#define xlcOptionsCalculation (318 | xlCommand)
#define xlcOptionsEdit (319 | xlCommand)
#define xlcOptionsView (320 | xlCommand)
#define xlcAddinManager (321 | xlCommand)
#define xlcMenuEditor (322 | xlCommand)
#define xlcAttachToolbars (323 | xlCommand)
#define xlcVbaactivate (324 | xlCommand)
#define xlcOptionsChart (325 | xlCommand)
#define xlcVbaInsertFile (328 | xlCommand)
#define xlcVbaProcedureDefinition (330 | xlCommand)
#define xlcRoutingSlip (336 | xlCommand)
#define xlcRouteDocument (338 | xlCommand)
#define xlcMailLogon (339 | xlCommand)
#define xlcInsertPicture (342 | xlCommand)
#define xlcEditTool (343 | xlCommand)
#define xlcGalleryDoughnut (344 | xlCommand)
#define xlcChartTrend (350 | xlCommand)
#define xlcPivotItemProperties (352 | xlCommand)
#define xlcWorkbookInsert (354 | xlCommand)
#define xlcOptionsTransition (355 | xlCommand)
#define xlcOptionsGeneral (356 | xlCommand)
#define xlcFilterAdvanced (370 | xlCommand)
#define xlcMailAddMailer (373 | xlCommand)
#define xlcMailDeleteMailer (374 | xlCommand)
#define xlcMailReply (375 | xlCommand)
#define xlcMailReplyAll (376 | xlCommand)
#define xlcMailForward (377 | xlCommand)
#define xlcMailNextLetter (378 | xlCommand)
#define xlcDataLabel (379 | xlCommand)
#define xlcInsertTitle (380 | xlCommand)
#define xlcFontProperties (381 | xlCommand)
#define xlcMacroOptions (382 | xlCommand)
#define xlcWorkbookHide (383 | xlCommand)
#define xlcWorkbookUnhide (384 | xlCommand)
#define xlcWorkbookDelete (385 | xlCommand)
#define xlcWorkbookName (386 | xlCommand)
#define xlcGalleryCustom (388 | xlCommand)
#define xlcAddChartAutoformat (390 | xlCommand)
#define xlcDeleteChartAutoformat (391 | xlCommand)
#define xlcChartAddData (392 | xlCommand)
#define xlcAutoOutline (393 | xlCommand)
#define xlcTabOrder (394 | xlCommand)
#define xlcShowDialog (395 | xlCommand)
#define xlcSelectAll (396 | xlCommand)
#define xlcUngroupSheets (397 | xlCommand)
#define xlcSubtotalCreate (398 | xlCommand)
#define xlcSubtotalRemove (399 | xlCommand)
#define xlcRenameObject (400 | xlCommand)
#define xlcWorkbookScroll (412 | xlCommand)
#define xlcWorkbookNext (413 | xlCommand)
#define xlcWorkbookPrev (414 | xlCommand)
#define xlcWorkbookTabSplit (415 | xlCommand)
#define xlcFullScreen (416 | xlCommand)
#define xlcWorkbookProtect (417 | xlCommand)
#define xlcScrollbarProperties (420 | xlCommand)
#define xlcPivotShowPages (421 | xlCommand)
#define xlcTextToColumns (422 | xlCommand)
#define xlcFormatCharttype (423 | xlCommand)
#define xlcLinkFormat (424 | xlCommand)
#define xlcTracerDisplay (425 | xlCommand)
#define xlcTracerNavigate (430 | xlCommand)
#define xlcTracerClear (431 | xlCommand)
#define xlcTracerError (432 | xlCommand)
#define xlcPivotFieldGroup (433 | xlCommand)
#define xlcPivotFieldUngroup (434 | xlCommand)
#define xlcCheckboxProperties (435 | xlCommand)
#define xlcLabelProperties (436 | xlCommand)
#define xlcListboxProperties (437 | xlCommand)
#define xlcEditboxProperties (438 | xlCommand)
#define xlcPivotRefresh (439 | xlCommand)
#define xlcLinkCombo (440 | xlCommand)
#define xlcOpenText (441 | xlCommand)
#define xlcHideDialog (442 | xlCommand)
#define xlcSetDialogFocus (443 | xlCommand)
#define xlcEnableObject (444 | xlCommand)
#define xlcPushbuttonProperties (445 | xlCommand)
#define xlcSetDialogDefault (446 | xlCommand)
#define xlcFilter (447 | xlCommand)
#define xlcFilterShowAll (448 | xlCommand)
#define xlcClearOutline (449 | xlCommand)
#define xlcFunctionWizard (450 | xlCommand)
#define xlcAddListItem (451 | xlCommand)
#define xlcSetListItem (452 | xlCommand)
#define xlcRemoveListItem (453 | xlCommand)
#define xlcSelectListItem (454 | xlCommand)
#define xlcSetControlValue (455 | xlCommand)
#define xlcSaveCopyAs (456 | xlCommand)
#define xlcOptionsListsAdd (458 | xlCommand)
#define xlcOptionsListsDelete (459 | xlCommand)
#define xlcSeriesAxes (460 | xlCommand)
#define xlcSeriesX (461 | xlCommand)
#define xlcSeriesY (462 | xlCommand)
#define xlcErrorbarX (463 | xlCommand)
#define xlcErrorbarY (464 | xlCommand)
#define xlcFormatChart (465 | xlCommand)
#define xlcSeriesOrder (466 | xlCommand)
#define xlcMailLogoff (467 | xlCommand)
#define xlcClearRoutingSlip (468 | xlCommand)
#define xlcAppActivateMicrosoft (469 | xlCommand)
#define xlcMailEditMailer (470 | xlCommand)
#define xlcOnSheet (471 | xlCommand)
#define xlcStandardWidth (472 | xlCommand)
#define xlcScenarioMerge (473 | xlCommand)
#define xlcSummaryInfo (474 | xlCommand)
#define xlcFindFile (475 | xlCommand)
#define xlcActiveCellFont (476 | xlCommand)
#define xlcEnableTipwizard (477 | xlCommand)
#define xlcVbaMakeAddin (478 | xlCommand)
#define xlcInsertdatatable (480 | xlCommand)
#define xlcWorkgroupOptions (481 | xlCommand)
#define xlcMailSendMailer (482 | xlCommand)
#define xlcAutocorrect (485 | xlCommand)
#define xlcPostDocument (489 | xlCommand)
#define xlcPicklist (491 | xlCommand)
#define xlcViewShow (493 | xlCommand)
#define xlcViewDefine (494 | xlCommand)
#define xlcViewDelete (495 | xlCommand)
#define xlcSheetBackground (509 | xlCommand)
#define xlcInsertMapObject (510 | xlCommand)
#define xlcOptionsMenono (511 | xlCommand)
#define xlcNormal (518 | xlCommand)
#define xlcLayout (519 | xlCommand)
#define xlcRmPrintArea (520 | xlCommand)
#define xlcClearPrintArea (521 | xlCommand)
#define xlcAddPrintArea (522 | xlCommand)
#define xlcMoveBrk (523 | xlCommand)
#define xlcHidecurrNote (545 | xlCommand)
#define xlcHideallNotes (546 | xlCommand)
#define xlcDeleteNote (547 | xlCommand)
#define xlcTraverseNotes (548 | xlCommand)
#define xlcActivateNotes (549 | xlCommand)
#define xlcProtectRevisions (620 | xlCommand)
#define xlcUnprotectRevisions (621 | xlCommand)
#define xlcOptionsMe (647 | xlCommand)
#define xlcWebPublish (653 | xlCommand)
#define xlcNewwebquery (667 | xlCommand)
#define xlcPivotTableChart (673 | xlCommand)
#define xlcOptionsSave (753 | xlCommand)
#define xlcOptionsSpell (755 | xlCommand)
#define xlcHideallInkannots (808 | xlCommand)
