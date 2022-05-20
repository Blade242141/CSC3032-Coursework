using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifferentLists {
    public List<String> list;
    public String name;

    public void AddToList(String str) {
        list.Add (str);
    }

    public List<String> GetList() {
        return this.list;
    }

    public DifferentLists(String str, List<String> newList) {
        this.name = str;
        this.list = newList;
    }
}
