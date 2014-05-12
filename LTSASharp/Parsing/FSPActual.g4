grammar FSPActual;

// // Parser

// 1. FSP description
fsp_description: fsp_definition+;

fsp_definition: constantDef
              | rangeDef
              | setDef
              | processDef
              | compositeDef
              | propertyDef
              | progressDef
              | menuDef
              | fluentDef
              | assertDef
              ;

// 1.1	Identifiers
//      constantIdent: UpperCaseIdentifier;
//      rangeIdent:    UpperCaseIdentifier;
//      setIdent:      UpperCaseIdentifier;
//      parameterIdent:UpperCaseIdentifier;
//      processIdent:  UpperCaseIdentifier;
//      propertyIdent: UpperCaseIdentifier;
//      progressIdent: UpperCaseIdentifier;
//      menuIdent:     UpperCaseIdentifier;
//      fluentIdent:   UpperCaseIdentifier;
//      assertIdent:   UpperCaseIdentifier;
//      variable: LowerCaseIdentifier;

// 1.2	Action Labels
actionLabel: (LowerCaseIdentifier | LSquare expression RSquare) (actionLabelTail)*;
actionLabelTail: Dot LowerCaseIdentifier | LSquare expression RSquare; // AUX Rule

actionLabels: (actionLabel | set | LSquare actionRange RSquare) (actionLabelsTail)*;
actionLabelsTail: Dot (actionLabel | set) | LSquare (actionRange | expression) RSquare /*| BackSlash set*/; // AUX Rule

actionRange: range
           | set
           | LowerCaseIdentifier Colon range
           | LowerCaseIdentifier Colon set
           ;
range: UpperCaseIdentifier
     | expression DotDot expression
     ;
set: UpperCaseIdentifier
   | LCurly setElements RCurly
   ;
setElements: actionLabels (Comma actionLabels)*;

// 1.3	const, range, set
constantDef: Const UpperCaseIdentifier Equal simpleExpression;
rangeDef: Range UpperCaseIdentifier Equal simpleExpression DotDot simpleExpression;
setDef: Set UpperCaseIdentifier Equal LCurly setElements RCurly;

// 1.4	Process Definition
processDef: UpperCaseIdentifier param? Equal processBody alphabetExtension? relabel? hiding? Dot;
processBody: localProcess (Comma localProcessDefs)?;
localProcessDefs: localProcessDef (Comma localProcessDef)*;
localProcessDef: UpperCaseIdentifier indexRanges? Equal localProcess;
alphabetExtension: Plus set;

localProcess: baseLocalProcess
			| sequentialComposition
            | If expression Then localProcess (Else localProcess)?
            | LRound choice RRound
            ;
baseLocalProcess: UpperCaseIdentifier indices?;
choice: actionPrefix (Or actionPrefix)*;
actionPrefix: guard? actionLabels Arrow (actionLabels Arrow)* localProcess;
guard: When expression;

indices: (LSquare expression RSquare)+;
indexRanges: (LSquare (expression|actionRange) RSquare)+;

sequentialComposition: seqProcessList Semicolon baseLocalProcess;
seqProcessList: processRef (Semicolon processRef)*;
processRef: UpperCaseIdentifier argument?;
argument: LRound argumentList RRound;
argumentList: expression (Comma expression)*;

// 1.5	Composite Process
compositeDef: OrOr UpperCaseIdentifier param? Equal compositeBody priority? hiding? Dot;
compositeBody: prefixLabel? processRef relabel?
             | prefixLabel? LRound parallelComposition RRound relabel?
             | ForAll ranges compositeBody  //replication
             | If expression Then compositeBody (Else compositeBody)? //conditional
             ;
prefixLabel: actionLabels Colon //process labeling
           | actionLabels ColonColon (actionLabels Colon)? //process sharing
           ;
parallelComposition: compositeBody (OrOr compositeBody)*;
priority: GtGt set
        | LtLt set
        ;
ranges: (LSquare actionRange RSquare)+;

// 1.6	Parameters
param: LRound parameterList RRound;
parameterList: parameter (Comma parameter)*;
parameter: UpperCaseIdentifier Equal expression;

// 1.7	Re-labeling and Hiding
relabel: Divide LCurly relabelDefs RCurly;
relabelDefs: relabelDef (Comma relabelDef)*;
relabelDef: actionLabels Divide actionLabels
          | ForAll indexRanges LCurly relabelDefs RCurly
          ;

hiding: BackSlash set
      | At set
      ;

// 1.8	Property, Progress and Menu
propertyDef: Property processDef;

progressDef: Progress UpperCaseIdentifier ranges? Equal (actionLabels | If set Then set); // SPEC says "= Set", IMPL says "= ActionLabels"

menuDef: Menu UpperCaseIdentifier Equal actionLabels; // SPEC says "= Set", IMPL says "= ActionLabels"

// 1.9	Expression
simpleExpression: additiveExpr;
expression: orExpr;
orExpr: andExpr (OrOr andExpr)*;
andExpr: bitOrExpr (AndAnd bitOrExpr)*;
bitOrExpr:  bitExclOrExpr (Or bitExclOrExpr)*;
bitExclOrExpr: bitAndExpr (Hat bitAndExpr)*;
bitAndExpr: equalityExpr (And equalityExpr)*;
equalityExpr: relationalExpr ((EqualEqual | NotEqual) relationalExpr)*;
relationalExpr: shiftExpr ((Lt | LtEqual | Gt | GtEqual) shiftExpr)*;
shiftExpr: additiveExpr ((LtLt | GtGt) additiveExpr)*;

additiveExpr: multiplicativeExpr ((Plus | Minus) multiplicativeExpr)*;
multiplicativeExpr: unaryExpr ((Star | Divide | Modulo) unaryExpr)*;
unaryExpr: (Plus | Minus | Not)? baseExpr;
baseExpr: IntegerLiteral
        | LowerCaseIdentifier
        | UpperCaseIdentifier
        | Quote actionLabel
        | Hash UpperCaseIdentifier
        | At LRound UpperCaseIdentifier Comma expression RRound
        | LRound expression RRound
        ;
      
// 1.10	Basic FSP
// Internal

// 1.11	fluent and assert
fluentDef: Fluent UpperCaseIdentifier indexRanges? Equal Lt actionLabels Comma actionLabels Gt initially?;
initially: Initially simpleExpression;

assertDef: Assert UpperCaseIdentifier param? Equal fltl_unary;

fltl_or: fltl_binary (OrOr fltl_binary)*;
fltl_binary: fltl_and
           | fltl_binary {CurrentToken.Text == "U" || CurrentToken.Text == "W" }? UpperCaseIdentifier fltl_and // until / weak until
           | fltl_binary Arrow   fltl_and // implication
           | fltl_binary Equiv fltl_and // equivalence
           ;
fltl_and: fltl_unary
        | fltl_and AndAnd fltl_unary
        ;
fltl_unary: fltl_base
          | Not fltl_unary        // negation
          | {CurrentToken.Text == "X" }? UpperCaseIdentifier fltl_unary // next time
          | Eventually fltl_unary        // eventually
          | Always fltl_unary        // always
          | ForAll ranges fltl_unary        
          | Exists ranges fltl_unary
          ;
fltl_base: UpperCaseIdentifier ranges?
         | actionLabels
         | UpperCaseIdentifier argument?
         | Rigid simpleExpression
         | LRound fltl_or RRound
         ;

// // Lexer

// Symbols
Dot: '.';
DotDot: '..';
LSquare: '[';
RSquare: ']';
Colon: ':';
LCurly: '{';
RCurly: '}';
Comma: ',';
Equal: '=';
Plus: '+';
LRound: '(';
RRound: ')';
Or: '|';
Arrow: '->';
Semicolon: ';';
OrOr: '||';
ColonColon: '::';
GtGt: '>>';
LtLt: '<<';
Divide: '/';
AndAnd: '&&';
Hat: '^';
And: '&';
EqualEqual: '==';
NotEqual: '!=';
Lt: '<';
LtEqual: '<=';
Gt: '>';
GtEqual: '>=';
Minus: '-';
Star: '*';
Modulo: '%';
Not: '!';
Quote: '\'';
Hash: '#';
At: '@';
BackSlash: '\\';
Equiv: '<->';
Eventually: '<>';
Always: '[]';

// Keywords
Const: 'const';
Range: 'range';
Set: 'set';
If: 'if';
Then: 'then';
Else: 'else';
//End: 'END';
//Stop: 'STOP';
//Error: 'ERROR';
When: 'when';
ForAll: 'forall';
Property: 'property';
Progress: 'progress';
Menu: 'menu';
Fluent: 'fluent';
Initially: 'initially';
Assert: 'assert';
Exists: 'exists';
Rigid: 'rigid';
Minimal: 'minimal' -> skip; //NOT IN SPEC, skipping until I found out what it does 
Compose: 'compose' -> skip; //NOT IN SPEC, skipping until I found out what it does 

// General
UpperCaseIdentifier: UpperCaseLetter (Letter|Digit)*;
LowerCaseIdentifier: LowerCaseLetter (Letter|Digit)*;
IntegerLiteral: Digit+;

fragment Letter: UpperCaseLetter | LowerCaseLetter;
fragment UpperCaseLetter: [A-Z];
fragment LowerCaseLetter: [a-z_];
fragment Digit: [0-9];

// Ignores

Whitespace: ( '\t' | ' ' | '\r' | '\n' ) -> skip;
SingleComment: '//' ~[\r\n]* -> skip;
MultiComment: '/*' .*? '*/' -> skip;