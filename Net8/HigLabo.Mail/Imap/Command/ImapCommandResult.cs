using System;
using System.Text;
using HigLabo.Net.Mail;

namespace HigLabo.Net.Imap;

/// <summary>Represents result of Imap command class.
/// </summary>
public class ImapCommandResult
{
    private byte[]? _Data = null;
    private ImapCommandResultStatus _Status = ImapCommandResultStatus.None;
    /// <summary>
    /// 
    /// </summary>
    public String Text
    {
        get { return Encoding.ASCII.GetString(_Data); }
    }
    public byte[]? Data
    {
        get { return _Data; }
    }
    /// <summary>
    /// 
    /// </summary>
    public ImapCommandResultStatus Status
    {
        get { return this._Status; }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="text"></param>
    public ImapCommandResult (string tag, byte[] arr)
    {
        if (arr == null || arr.Length == 0 || string.IsNullOrEmpty(tag))
            return;
        
        _Data = arr;
        // Last line
        int lineStart = FindLastLineStart(arr);
        if (lineStart < 0)
            return;
        
        // tag to bytes
        byte[] tagBytes = Encoding.ASCII.GetBytes(tag);
        
        // End with "tag"
        for (int i = 0; i < tagBytes.Length; i++)
        {
            if (lineStart + i >= arr.Length || arr[lineStart + i] != tagBytes[i])
                return ;
        }
        
        // space tag after
        int pos = lineStart + tagBytes.Length;
        if (pos >= arr.Length || arr[pos] != ' ')
            return ;
        
        pos++; // skip space
        
        // OK
        if (pos + 1 < arr.Length)
        {
            if ((arr[pos] == 'O' || arr[pos] == 'o') && 
                (arr[pos + 1] == 'K' || arr[pos + 1] == 'k'))
            {
                this._Status = ImapCommandResultStatus.Ok;
                return ;
            }
            
            // NO
            if ((arr[pos] == 'N' || arr[pos] == 'n') && 
                (arr[pos + 1] == 'O' || arr[pos + 1] == 'o'))
            {
                this._Status = ImapCommandResultStatus.No;
                return ;
            }
            
            // BAD
            if (pos + 2 < arr.Length &&
                (arr[pos] == 'B' || arr[pos] == 'b') && 
                (arr[pos + 1] == 'A' || arr[pos + 1] == 'a') &&
                (arr[pos + 2] == 'D' || arr[pos + 2] == 'd'))
            {
                this._Status = ImapCommandResultStatus.Bad;
                return ;
            }
        }
    }

    private int FindLastLineStart(byte[] arr)
    {
        // End last line
        int pos = arr.Length - 1;
        while (pos >= 0 && (arr[pos] == '\n' || arr[pos] == '\r'))
        {
            pos--;
        }
        
        if (pos < 0)
            return -1;
        
        // Start line
        while (pos >= 0 && arr[pos] != '\n' && arr[pos] != '\r')
        {
            pos--;
        }
        
        return pos + 1;
    }
}
