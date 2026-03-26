using System;
using System.Collections.Generic;
using System.Text;
using HigLabo.Net.Mail;
using HigLabo.Net.Imap;

namespace HigLabo.Net.Internal;

/// <summary>
/// Represent context of request and response process and provide data about context.
/// </summary>
public class ImapDataReceiveContext : DataReceiveContext
{
    private enum ParseState
    {
        TagValidating, MultiLine, CarriageReturn, LastLine, LastLineCarriageReturn, 
    }
    private Byte[] _TagBytes;
    //  tag1 can be split between read attempt into buffer
    private int _tagIndex = 0;
    private ParseState _State = ParseState.TagValidating;

    public Boolean IsFetchCommand { get; set; }

    internal ImapDataReceiveContext(String tag, Encoding encoding) 
        :base(encoding)
    {
        _TagBytes = this.Encoding.GetBytes(tag);
        this.IsFetchCommand = false;
    }

    /// <summary>
    /// Read buffer data to Data property and initialize buffer.
    /// If response has next data,return true.
    /// </summary>
    /// <param name="size"></param>
    /// <returns>If response has next data,return true</returns>
    protected override Boolean ParseBuffer(Int32 size)
    {
        Byte[] bb = this.Buffer;

        for (int i = 0; i < size; i++)
        {
            this.Stream.WriteByte(bb[i]);
            if (_State == ParseState.TagValidating)
            {
                if (bb[i] == _TagBytes[_tagIndex])
                {
                    _tagIndex = _tagIndex + 1;
                    if (_TagBytes.Length == _tagIndex)
                    {
                        _State = ParseState.LastLine;
                    }
                }
                else
                {
                    _State = ParseState.MultiLine;
                }
            }
            else if (_State == ParseState.MultiLine)
            {
                if (bb[i] == AsciiCharCode.CarriageReturn.GetNumber())
                {
                    _State = ParseState.CarriageReturn;
                }
            }
            else if (_State == ParseState.CarriageReturn)
            {
                if (bb[i] == AsciiCharCode.LineFeed.GetNumber())
                {
                    _tagIndex = 0;
                    _State = ParseState.TagValidating;
                }
                else if (bb[i] == AsciiCharCode.CarriageReturn.GetNumber())
                {
                    _State = ParseState.CarriageReturn;
                }
                else
                {
                    _tagIndex = 0;
                    _State = ParseState.TagValidating;
                }
            }
            else if (_State == ParseState.LastLine)
            {
                if (bb[i] == AsciiCharCode.CarriageReturn.GetNumber())
                {
                    _State = ParseState.LastLineCarriageReturn;
                }
            }
            else if (_State == ParseState.LastLineCarriageReturn)
            {
                if (bb[i] == AsciiCharCode.LineFeed.GetNumber())
                {
                    if (i == size -1) //at end of buffer
                        return false;
                    else
                    {
                        _State = ParseState.TagValidating;
                        _tagIndex = 0;
                    }
                }
                else { throw new DataTransferContextException(this); }
            }
            bb[i] = 0;
        }
        return true;
    }
}
