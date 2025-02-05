namespace Petit.Lexer
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
        /// if
        /// </summary>
        If,

        /// <summary>
        /// else
        /// </summary>
        Else,

        /// <summary>
        /// return
        /// </summary>
        Return,

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
