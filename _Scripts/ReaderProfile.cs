using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ReaderProfileData
{
    public uint ReaderId;
    public string ReaderName;
    public string ReaderEmail;
    [Range(0f, Globals.MAX_READER_RATING)]
    public float ReaderRating;
    public bool IsSuperUser;

    public List<BookListItem> ActiveBooks;

    public ReaderProfileData(ReaderProfile readerProfile)
    {
        ReaderId = readerProfile.ReaderId;
        ReaderName = readerProfile.ReaderName;
        ReaderEmail = readerProfile.ReaderEmail;
        ReaderRating = readerProfile.ReaderRating;
        IsSuperUser = readerProfile.IsSuperUser;

        ActiveBooks = new List<BookListItem>();

        foreach (var pair in readerProfile.ActiveBooksDict)
        {
            pair.Value.UpdateDateStr();
            ActiveBooks.Add(pair.Value);
        }
    }
}

[Serializable]
public struct BookListItem
{
    public uint BookId;
    public DateTime DeadlineTime;
    public bool IsOverdue;
    public string DeadlineTimeStr;

    public BookListItem(uint bookId, DateTime deadlineTime, bool isOverdue)
    {
        BookId = bookId;
        DeadlineTime = deadlineTime;
        IsOverdue = isOverdue;
        DeadlineTimeStr = DeadlineTime.ToShortDateString();
    }

    public void UpdateDateStr()
    {
        DeadlineTimeStr = DeadlineTime.ToShortDateString();
    }

    public void UpdateDate()
    {
        DeadlineTime = DateTime.Parse(DeadlineTimeStr);
    }
}

[Serializable]
public class ReaderProfile
{
    public uint ReaderId;
    public string ReaderName;
    public string ReaderEmail;
    [Range(0f, Globals.MAX_READER_RATING)]
    public float ReaderRating;
    public bool IsSuperUser;
    public Dictionary<uint, BookListItem> ActiveBooksDict = new Dictionary<uint, BookListItem>();
    

    //public ReaderProfile(uint readerId, string readerName, string rearedEmail, float readerRating, bool isSuperUser, Dictionary<uint, BookListItem> activeBooks)
    public ReaderProfile(uint readerId, string readerName, string rearedEmail, float readerRating, bool isSuperUser, Dictionary<uint, BookListItem> activeBooksDict)
    {
        ActiveBooksDict.Clear();
        ReaderId = readerId;
        ReaderName = readerName;
        ReaderEmail = rearedEmail;
        ReaderRating = readerRating;
        IsSuperUser = isSuperUser;
        ActiveBooksDict = activeBooksDict;
    }

    public ReaderProfile(ReaderProfileData data)
    {
        ReaderId = data.ReaderId;
        ReaderName = data.ReaderName;
        ReaderEmail = data.ReaderEmail;
        ReaderRating = data.ReaderRating;
        IsSuperUser = data.IsSuperUser;
        ActiveBooksDict = new Dictionary<uint, BookListItem>();

        foreach(var dataItem in data.ActiveBooks)
        {
            dataItem.UpdateDate();
            ActiveBooksDict.Add(dataItem.BookId, dataItem);
        }
    }

    public void AddActiveBook(Book book, DateTime deadlineTime)
    {
        if (deadlineTime > DateTime.Now)
        {
            Debug.Log($"Book with id: {book.BookId} added as active book to Reader Profile with id {this.ReaderId}");
            var listItem = new BookListItem(book.BookId, deadlineTime, false);
            if(ActiveBooksDict == null)
            {
                ActiveBooksDict = new Dictionary<uint, BookListItem>();
            }
            ActiveBooksDict.Add(book.BookId, listItem);
        }
        else
        {
            Debug.LogError($"Cannot add Book with id: {book.BookId} as active book to Reader Profile with id {this.ReaderId}\nDeadline is invalid");
        }
    }

    public void RemoveActiveBook(Book book) 
    {
        if (ActiveBooksDict.TryGetValue(book.BookId, out BookListItem bookListItem))
        {
            Debug.Log($"Book with id: {book.BookId} removed as active book from Reader Profile with id {this.ReaderId}");
            ActiveBooksDict.Remove(book.BookId);
        }
        else
        {
            Debug.LogError($"Cannot remove Book with id: {book.BookId} as active book from Reader Profile with id {this.ReaderId}\nCan't find book in container");
        }
    }

    public void UpdateBooksStatus()
    {
        foreach (var pair in ActiveBooksDict)
        {
            var item = pair.Value;
            if (item.DeadlineTime > DateTime.Now)
            {
                item.IsOverdue = true;
                ActiveBooksDict[pair.Key] = item;
            }
        }
    }

    public ReaderProfileData SaveData()
    {
        var data = new ReaderProfileData(this);
        Debug.Log($"Converted Reader Profile with id: {ReaderId} data to struct");
        return data;
    }
}
