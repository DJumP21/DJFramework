using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
/// <summary>
/// BundleÁ÷
/// </summary>
public class BundleStream : FileStream
{
    public const byte KEY = 64;

    public BundleStream(string path,FileMode mode,FileAccess access , FileShare share):base (path,mode,access,share)
    {

    }

    public BundleStream(string path,FileMode mode) : base(path,mode) 
    {

    }

    public override int Read(byte[] array, int offset, int count)
    {
        int index = base.Read(array, offset, count);
        for(int i=0;i<array.Length;i++)
        {
            array[i] ^= KEY;
        }
        return index;
    }
}
