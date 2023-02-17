# Playen
## An interest-based English words memorizing app

### Basic Data Structures

```json lines
// Vocabulary -- List[Word]
// Word
{
    "word": "abandon",
    "familiar_degree": 0(0-4)
}
// Word Details -- List[WordDetail]
// Word Detail
{
    "word": "abandon",
    "word_translation": "v.抛弃...", 
    "contents": [
        {
            "speaker": "Jack",
            "speakerColor": "Black",
            "content": "...",
            "translation": "..."
        }
    ] 
}
```

### Classes And Exposing APIs

1. Local Storage Class

   Local Storage stores vocabulary and word details getting from Remote Storage(which we will talk about later). For its user, class Local Storage exposes 6 functions: **Init**, **Current**, **MoveNext**, **Repeat**, **Detail**, **Clear**. Frankly speaking, our local storage class behaves just like a linked list, but with more complex functions.

   ```c#
   /* 
    * Init -- initialize local storage. This function should be used
    * after getting Userkey(sign in).
    * retval: true -- Success! You can now start to use local storage.
    *         false -- Sorry! Maybe some error occurs.
   */
   public bool Init();
   
   /*
    * Current -- return current word. 
    * To make it easy to use, we use an auto-property
    * retval: type: Word.
    */
   public Word Current
   {
       get => ...;
   }
   
   /*
    * Next -- move to next word.
    * retval: true -- succeed to move to next
    *         false -- this is the last word, moving failed
    */
   public bool MoveNext();
   
   /* 
    * Repeat -- repeat current word.
    * retval: null. This function always run successfully
    */
   public void Repeat();
   
   /* 
    * Detail -- get Word Detail for this word
    * also, we make it an auto-property
    * retval: WordDetail
    */
   public WordDetail Detail
   {
       get => ...;
   }
   
   /*
    * Clear -- use when user alters. After Clear(), you should use 
    * Init() again to reinitailize local storage
    */
   public void Clear();
   ```

   

   

### Client GUI Design

GUI means Graphical User Interface. Because a user will directly interact with GUI, it is usually the most important part in APP designing and programming. Here, we use normal designing software(Like Photoshop, Axure) to  design and **Xamarin** (a useful programming framework designed by Microsoft and wriiten with C#) to program.

1. Main Page

   The main page is the most basic page in an app. A carefully-designed main page will enable the user to find all functions conveniently. In this app, the main page can be divided to three parts -- **Index Page** and **Login&Register Page**. The three pages can alter through the tab at the bottom of the screen.

2. Index Page

   The index page is the first page for user to see after starting the app, and it's also the page to enter the most important function of the app. For Playen, of course, you can start your learning by simply clicking a "START" bottom at this **Index Page**. But before then, you should login in at the **Login&Register Page**.

3. Login&Register Page

   Account page, just like its name, is where you manage your account. For our simple app, we only provide two basic managements -- logging and registering. When you first login in, the user name and the password (encrypted) will be stored in the database -- which means registering a new account -- and next time you can login in with them. After signing in, your learning progress can be stored in the database and you can resume your learning wherever you are.

4. Word Page & Word Detail Page

   Inspired by some other apps, we design two types of pages to help you to pay more attention to the words you are not familiar with. **Word Page** is a page with only a English word and two choices: Known, Unknown. When the user is familiar with the word, he/she can choose "Known", and then this word will be skipped. On the other hand, if they are not familiar with the word, they can choose "Unknown", and they will go to the **Word Detail Page** of the word.

   The Word Detail Page contains more information about this word: the word itself, the translation of the word, a short paragraph providing a specific context for the word. Because our paragraphs are extracted from game, they are usually dialogues, so before the sentence, we put the speakers' name in a specific color.

   During designing, we considered to extract records from game and add them to our app. However, because we need to use record-to-text software to match the record and the text, while most of them are not cheap to use, we gave up this designing in the first version. We may add it to the app in next update.

### Features

#### Current Features

1. Learning and Reviewing

   **Learning Process**

   + ==Groups==: We seperate the total list to groups smaller than 7 words. For instance, if we plan to learn 20 once, we can devide it to 20=7+7+6
   + ==Skip==: if you choose "known" when the familiar of word is 0, you can skip the word and you won't see it later.
   + ==Repeat==: if you choose "unknown", the familiar will reset to 0, and you will see it when you are learning the rest groups of words.(if there's no more groups, a new group will be added); on the other hand, even if you choose "known" when the familiar degree is not 0, you will see the word later, but, differently, the familiar degree will not reset.

   > Eg. 
   >
   > First divide words to A1-A7, B1-B7,C1-C6; 
   >
   > Next, show words A1-A7(first group), assuming that the user skips word A6, A7. 
   >
   > In the second group, we first show B1-B7, then we show A1-A5(repeat), assuming that the user skips A1-A3, B1-B5.
   >
   > In the third group, we show C1-C6 first, then we show A1-A5,B6,B7(repeat), assuming that the user skips A1-A5, B6, C1-C5.
   >
   > At last, all words on the list have appeared. Now, the familiar degree for each word: A1-A3: 2, A4, A5, B6: 1, C6, B7: 0. So we show A1 A2 A3 A4 A5 B6 B7 C6 -> until all familiar degree reach maximum.

   We use an Enumerable-Enumerator structure to deal with this problem. We create a class named "WordQueue" derived from class IEnumerable, meaning that it can generate an instance of class IEnumerator. So the user can get all data through the enumerator without knowing the concrete structure of data.

   We use a 2-dimensional list to store all words we need to learn. The first dimension means group, and the second dimension means how words are organized in a group. That makes the problem easy to solve. Repeating means adding word at the tail of next group's list.

2. Encrypt & Decrypt

   + (password + salt -> SHA256) -> HTTPS -> database
   + password -> BCrypt -> HTTPS -> database

3. Remember Pwd & Auto Login

   It is not safe to directly store the password in the device. Learning from web, I think these two functions should contains several following steps.

   + When login succeeds and "Remember My Pwd" is chosen, client should ask the server for a token, this token should be stored in local storage. Also, to make it seems that the password is remembered, we can also store the length of the password and the username.
   + Each time when we use the app, we read the token, username, length of the password from local storage. We fill the textbox of the login page with real username and fake password(generate according to the length).
   + If "Auto Login" is also chosen last time when we logined in, the token will be sent to the server, which returns an updated Userkey and an updated Token.
   + If not, when the user clicked login, the token will be sent, which will get a same result as above.

4. MVVM Structure - Model, ViewModel, View - To Change Text With Variable Automatically

   Here's a example:

   ```xaml
   <!-- View -->
   <Label Grid.Row="3" Grid.Column="0" 
          HorizontalOptions="Center"
          Text="{Binding WordsToStudy, StringFormat='{0}'}" 
          FontSize="20" 
          FontAttributes="Bold"
          TextColor="Black">
   ```

   Here we bind Property **Text** to **BindingContext.WordsToStudy**

   ```C#
   // ViewModel
   class IndexViewModel : INotifyPropertyChanged
       {
           private int _wordsToStudy;
           public event PropertyChangedEventHandler PropertyChanged;
   
           public IndexViewModel()
           {
               _wordsToStudy = GlobalClasses.LearningWordsCount;
           }
   
           public int WordsToStudy
           {
               get => _wordsToStudy;
               set
               {
                   if (_wordsToStudy != value)
                   {
                       _wordsToStudy = value;
   
                       if (PropertyChanged != null)
                       {
                           PropertyChanged(this, 
                            new PropertyChangedEventArgs("WordsToStudy"));
                       }
                   }
               }
           }
       }
   ```

   ViewModel will be bound to View automatically by Xamarin. Next, we can set it to BindingContext in Page.xaml.cs

   ```C#
   // Index.xaml.cs
   private int _wordsToStudy;
   private readonly IndexViewModel IndexViewModel = null;
   BindingContext = IndexViewModel = new IndexViewModel();
   
   public int WordsToStudy
   {
       get => _wordsToStudy;
       set
       {
           if (_wordsToStudy != value)
           {
               _wordsToStudy = value;
               IndexViewModel.WordsToStudy = value;
           }
       }
   }
   ```

5. Pause And Continue

   We place a "pause and exit" button to enable user to get out of the confirm page and detail page. And we set that only after all words are finished will we get new groups of words from server. We will also write the WordQueue into local file to get it after exiting the app.

#### Future Features We May Add

### Client-Server APIs

> Please see: Playen APIs Web Document
>
> [Playen APIs]: https://www.cloud-smx2003.fun/apidocs

### Genshin Theme

#### Colors For All Speakers

**Characters**

Venti(#359697), Zhongli(#443d38),Yun Jin(4a385f),Yoimiya(f4d6ba)宵宫,Yelan(5766a6), Yanfei(f0b09a), Yae Miko(ecadb0) 八重神子, Xinyan(c79e88), Xingqiu(467c99),Xiao(539ca0), Xiangling(d6634c),Traveller(faebbc), Tighnari(193845) 提纳里, Thoma(eeba86) 托马, Sucrose(e8f5d8) 砂糖, Shenhe(eff5f9), Sayu(8aa153) 早柚, Sara(50546e) 九条裟罗, Rosaria(893f66) 罗莎莉亚, Razor(d3dadf) 雷泽, Raiden(473980) 雷电将军, Qiqi(d3d0f0), Noelle(b59982) 诺艾尔, Ningguang(f9f5ec), Nilou(a4bbcb) 妮露, Nahida(9fca59) 纳西妲, Mona(5a4a89), Lisa(302257), Kuki Shinobu(a8c485) 久岐忍, Kokomi(fadcd4) 心海, Klee(a0301b) 可莉, Keqing(a499b3), Kazuha(c24625) 枫原万叶, kaeya(234660) 凯亚, Jean(ecd7c1) 琴, Itto(41342f) 一斗, Hu Tao(cb3b33), Heizou(773346) 鹿野院平藏,  Gorou(a57d46) 五郎, Ganyu(d3dff0), Fischl(9b7ed5) 菲谢尔,  Eula(aacfd6) 优菈,  Dori(f0c0c9) 多莉, Diona(f7bfbe) 迪奥娜, Diluc(cc463a) 迪卢克, Cyno(6d6ec6) 赛诺, Collei(bac689) 柯莱, Chongyun(b8e3ea), Childe(ce7c3a) 达达利亚, Candace(494675) 坎蒂丝, Bennett(fef9ee) 班尼特, Beidou(3d2a2f), Babara(d7c8b3), Ayato(a5b9e3) 神里绫人, Ayaka(ecf2f8)  神里绫华, Amber(a4191d) 安柏, Aloy(c0704e) 埃洛伊, Albedo(fffbf0) 阿贝多, Layla(5c6287) 莱伊拉

**NPCs**

**Signs**
