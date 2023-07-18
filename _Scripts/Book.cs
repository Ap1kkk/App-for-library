using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Book 
{
    public uint BookId;
    public string Title;
    public string Author;
    public string Genre;

    public Book(uint bookId, string title, string author, string genre)
    {
        BookId = bookId;
        Title = title;
        Author = author;
        Genre = genre;
    }
}
