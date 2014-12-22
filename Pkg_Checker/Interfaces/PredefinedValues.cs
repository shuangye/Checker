using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pkg_Checker.Interfaces
{
    [Flags]
    public enum CommentCheckResult
    {
        // comments checking:
        COMMENT_OK = 0,
        COMMENT_STICKYNOTE_EMPTY = 1 << 0,
        COMMENT_REPLYTO_STICKYNOTE_EMPTY = 1 << 1,
        COMMENT_NO_MODERATOR_STAMP = 1 << 2
    }
}
