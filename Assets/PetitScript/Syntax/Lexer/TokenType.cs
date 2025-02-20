namespace Petit.Syntax.Lexer
{
    enum TokenType
    {
        /// <summary>
        /// 無効値
        /// </summary>
        Invalid,

        /// <summary>
        /// バックスラッシュ
        /// </summary>
        BackSlash,

        /// <summary>
        /// (
        /// </summary>
        LParen,

        /// <summary>
        /// )
        /// </summary>
        RParen,

        /// <summary>
        /// {
        /// </summary>
        LBrace,

        /// <summary>
        /// }
        /// </summary>
        RBrace,

        /// <summary>
        /// [
        /// </summary>
        LBracket,

        /// <summary>
        /// ]
        /// </summary>
        RBracket,

        /// <summary>
        /// "
        /// </summary>
        DoubleQuote,

        /// <summary>
        /// 識別子
        /// </summary>
        Ident,

        /// <summary>
        /// 値
        /// </summary>
        Value,

        /// <summary>
        /// :
        /// </summary>
        Colon,

        /// <summary>
        /// ;
        /// </summary>
        Semicolon,

        /// <summary>
        /// ,
        /// </summary>
        Comma,

        /// <summary>
        /// .
        /// </summary>
        Dot,

        /// <summary>
        /// =
        /// </summary>
        Assign,

        /// <summary>
        /// !
        /// </summary>
        Not, 

        /// <summary>
        /// ==
        /// </summary>
        Equals,

        /// <summary>
        /// ===
        /// </summary>
        Identical,

        /// <summary>
        /// !=
        /// </summary>
        NotEquals,

        /// <summary>
        /// !==
        /// </summary>
        NotIdentical,

        /// <summary>
        /// <
        /// </summary>
        LessThan,

        /// <summary>
        /// <=
        /// </summary>
        LessThanOrEquals,

        /// <summary>
        /// >
        /// </summary>
        GreaterThan,

        /// <summary>
        /// >=
        /// </summary>
        GreaterThanOrEquals,

        /// <summary>
        /// <=>
        /// </summary>
        Spaceship,

        /// <summary>
        /// &&
        /// </summary>
        LogicalAnd,
        
        /// <summary>
        /// ||
        /// </summary>
        LogicalOr,

        /// <summary>
        /// &
        /// </summary>
        BitwiseAnd,

        /// <summary>
        /// |
        /// </summary>
        BitwiseOr,

        /// <summary>
        /// ^
        /// </summary>
        BitwiseXor,

        /// <summary>
        /// ~
        /// </summary>
        BitwiseNot,

        /// <summary>
        /// <<
        /// </summary>
        ShiftLeft,

        /// <summary>
        /// >>
        /// </summary>
        ShiftRight,

        /// <summary>
        /// +
        /// </summary>
        Add,

        /// <summary>
        /// -
        /// </summary>
        Sub,

        /// <summary>
        /// *
        /// </summary>
        Mul,

        /// <summary>
        /// /
        /// </summary>
        Div,

        /// <summary>
        /// %
        /// </summary>
        Mod,

        /// <summary>
        /// ?
        /// </summary>
        Question,

        /// <summary>
        /// +=
        /// </summary>
        AddAssign,

        /// <summary>
        /// -=
        /// </summary>
        SubAssign,

        /// <summary>
        /// *=
        /// </summary>
        MulAssign,

        /// <summary>
        /// /=
        /// </summary>
        DivAssign,

        /// <summary>
        /// /=
        /// </summary>
        ModAssign,

        /// <summary>
        /// <<=
        /// </summary>
        ShiftLeftAssign,

        /// <summary>
        /// >>=
        /// </summary>
        ShiftRightAssign,

        /// <summary>
        /// ++
        /// </summary>
        Inc,

        /// <summary>
        /// --
        /// </summary>
        Dec,

        /// <summary>
        /// &=
        /// </summary>
        BitwiseAndAssign,

        /// <summary>
        /// |=
        /// </summary>
        BitwiseOrAssign,

        /// <summary>
        /// ^=
        /// </summary>
        BitwiseXorAssign,

        /// <summary>
        /// if
        /// </summary>
        If,

        /// <summary>
        /// else
        /// </summary>
        Else,

        /// <summary>
        /// switch
        /// </summary>
        Switch,

        /// <summary>
        /// case
        /// </summary>
        Case,

        /// <summary>
        /// default
        /// </summary>
        Default,

        /// <summary>
        /// while
        /// </summary>
        While,

        /// <summary>
        /// break
        /// </summary>
        Break,

        /// <summary>
        /// continue
        /// </summary>
        Continue,

        /// <summary>
        /// for
        /// </summary>
        For,

        /// <summary>
        /// return
        /// </summary>
        Return,

        /// <summary>
        /// fn (function)
        /// </summary>
        Fn,

        /// <summary>
        /// 改行
        /// </summary>
        NewLine,

        /// <summary>
        /// エイリアス
        /// </summary>
        Plus = Add,
        Minus = Sub,
        Slash = Div,
    }
}
