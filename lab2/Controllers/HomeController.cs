/*
  Name: Hasan Senyurt
  Index No: K-6644
  Mail: 01176693@pw.edu.pl

    Newtonsoft.Json packet is used to handle .json files. 'dotnet add package Newtonsoft.Json' code must be written on terminal of
    visual studio code if it is not installed. .json files are inside of Models folder.
*/


using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using lab2.Models;
using Newtonsoft.Json; //''  dotnet add package Newtonsoft.Json  ''  please write this on Visual Studio Code Terminal


namespace lab2.Controllers;


//User class to save user information to json file.
public class User{
        public string userId {get; set;}
        public string email {get; set;}
        public string password {get; set;}
    }


//Blog class to save blog information to json file.
public class Blog{
        public string blogId {get; set;}
        public string title {get; set;}
        public string ownerId {get; set;}
        public DateTime datetime {get; set;}
        public string content {get; set;}

        
    }
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Register()
    {
        return View();
    }

    public IActionResult LogIn()
    {
        return View();
    }


    //sign out action for sign out button.
    public IActionResult SignOut()
    {
        return RedirectToAction("LogIn");
    }


    //login part of the system.
    public IActionResult LoginBlogMain(string loginuser, string loginpassword )
    {
        bool control = false;
        try{
            if(string.IsNullOrEmpty(loginuser) || string.IsNullOrEmpty(loginpassword)){

                ViewBag.Error = "Invalid user id or password. Please try again!";
                return View("Error");

            }
            else{

                //json
                string jsonData = System.IO.File.ReadAllText("Models/users.json");
                List<User> jsonRead = JsonConvert.DeserializeObject<List<User>>(jsonData);
                
                //checking if username and password is correct by searching it in json file.
                foreach(var item in jsonRead){

                    if(item.userId == loginuser && item.password == loginpassword){
                        return BlogMain(loginuser);
                    }

                    else{
                        control = true;
                    }
                }
            }
        }
        catch{
            ViewBag.Error = "Invalid user id or password. Please try again!";
            return View("Error");
        }


        if(control){
            ViewBag.Error = "Invalid user id or password. Please try again!";
            return View("Error");
        }
        else{
            return BlogMain(loginuser);
        }
    }


    //registration part of the system.
    public IActionResult RegisterBlogMain(string registeruser, string registeremail, string registerpassword ){

        //creating new user.
        User user = new User();
        user.userId = registeruser;
        user.email = registeremail;
        user.password = registerpassword;

        if(string.IsNullOrEmpty(registeruser) || string.IsNullOrEmpty(registeremail) || string.IsNullOrEmpty(registerpassword)){
            ViewBag.Error = "Invalid user id, email or password. Please try again!";
            return View("Error");
        }

        List<User> userJson = new List<User>();
        
        try{

            //json.
            string jsonData2 = System.IO.File.ReadAllText("Models/users.json");
            List<User> jsonRead = JsonConvert.DeserializeObject<List<User>>(jsonData2);


            //checking if there is someone who has the same id.
            foreach(var item in jsonRead){

                if(registeruser == item.userId){
                    ViewBag.Error = "There is someone who has the same user id. Please try new user id.";
                    return View("Error");
                }

                else{
                    userJson.Add(item);
                }
            }
        }
        catch{
        }
        
        userJson.Add(user);
        string jsonData = JsonConvert.SerializeObject(userJson);
        System.IO.File.WriteAllText("Models/users.json",jsonData);
        return BlogMain(registeruser);

    }


    // This function is for loading blogs on main screen. After every operation, blog list will be refreshed by this function.
    public string[] WriteBlogOnList(string blogOwner){
        
        List<Blog> blogJson = new List<Blog>();
        List<string> blogs = new List<string>();

        try{

            string jsonData2 = System.IO.File.ReadAllText("Models/blogs.json");
            List<Blog> jsonRead = JsonConvert.DeserializeObject<List<Blog>>(jsonData2);

            foreach(var item in jsonRead){

                if(item.ownerId == blogOwner){
                    blogs.Add("BLOG ID: " +item.blogId + ">>>> BLOG TITLE: " +item.title + ">>>> OWNER ID: " + item.ownerId + ">>>> PUBLISH DATE: " + item.datetime);
                }
            }
        }
        catch{
        }

        string[] blogArray = blogs.ToArray();
        return blogArray;
    }


    //main screen after login or registration
    public IActionResult BlogMain(string userid){

        ViewBag.UserId = userid; 
        ViewBag.Blogs = WriteBlogOnList(userid); //getting blog list.

        // Remove button is enabled or disabled according to number of blogs in the blog list.
        ViewBag.RemoveButton = "disabled";

        if(WriteBlogOnList(userid).Count() != 0){
            ViewBag.RemoveButton = "enabled";
        }

        return View("BlogMain");
    }


    //Viewing the selected blog.
    public IActionResult BlogView(string blogselect)
    {
        //string manipulation for the title of the blog.
        try{
            string[] words = blogselect.Split(">>>> ");
            int length = words[1].Count() - 12;
            ViewBag.Message = words[1].Substring(12, length);
        }

        catch{
            ViewBag.Error = "Please select one of the blog!";
            return View("Error");
        } 
        
        bool control = false;

        try{

            if(string.IsNullOrEmpty(blogselect)){
                ViewBag.Error = "Please select one of the blog!";
                return View("Error");
            }

            else{

                //json
                string jsonData = System.IO.File.ReadAllText("Models/blogs.json");
                List<Blog> jsonRead = JsonConvert.DeserializeObject<List<Blog>>(jsonData);
                

                //getting the content of selected blog from json file.
                foreach(var item in jsonRead){

                    if("BLOG ID: " +item.blogId + ">>>> BLOG TITLE: " +item.title + ">>>> OWNER ID: " + item.ownerId + ">>>> PUBLISH DATE: " + item.datetime== blogselect){
                        ViewBag.Message2 = item.content;
                        return View("BlogView");
                        
                    }
                    else{
                        control = true;
                    }
                }
            }
        }
        catch{
            ViewBag.Error = "There is no blog such as "+blogselect;
            return View("Error");
        }

        if(control){
            ViewBag.Error = "There is no blog such as "+blogselect;
            return View("Error");
        }
        else{
            return View("BlogView");
        }
    }


    //blog adding window.
    public IActionResult BlogAdd(string userid)
    {
        ViewBag.user = userid;
        return View();
    }

    //adding new blog (the last add blog button).
    public IActionResult BlogAddList(string ownerid, string blogid, string title, string content)
    {
        //create new blog to add.
        Blog blog = new Blog();
        blog.blogId = blogid;
        blog.title = title;
        blog.content = content;
        blog.ownerId = ownerid;
        blog.datetime = DateTime.Now;

        if(string.IsNullOrEmpty(blogid) || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(content)){
            ViewBag.Error = "Blog Id, Title or Content cannot be empty. Please fill them.";
            return View("Error");
        }

        List<Blog> blogJson = new List<Blog>();
        
        try{

            //json
            string jsonData2 = System.IO.File.ReadAllText("Models/blogs.json");
            List<Blog> jsonRead = JsonConvert.DeserializeObject<List<Blog>>(jsonData2);

            //checking if there is some blog which has the same id.
            foreach(var item in jsonRead){

                if(blogid == item.blogId){
                    ViewBag.Error = "There is a blog which has the same blog id. Please try different blog id.";
                    return View("Error");
                }
                else{
                    blogJson.Add(item);
                }
            }
        }
        catch{
        }
        
        blogJson.Add(blog);

        string jsonData = JsonConvert.SerializeObject(blogJson);
        System.IO.File.WriteAllText("Models/blogs.json",jsonData);
        return BlogMain(ownerid);
    }


    // Function to remove selected blog on blog list. 
    public IActionResult BlogRemove(string userid3,string blogselect){

        bool control = false;

        try{

            if(string.IsNullOrEmpty(blogselect)){
                ViewBag.Error = "Please select one of the blog!";
                return View("Error");
            }
            else{
                
                //json deserialize

                string jsonData = System.IO.File.ReadAllText("Models/blogs.json");
                List<Blog> jsonRead = JsonConvert.DeserializeObject<List<Blog>>(jsonData);
                
                //finding selected blog in json to remove it.
                foreach(var item in jsonRead){

                    if("BLOG ID: " +item.blogId + ">>>> BLOG TITLE: " +item.title + ">>>> OWNER ID: " + item.ownerId + ">>>> PUBLISH DATE: " + item.datetime == blogselect){
                        
                        jsonRead.Remove(item);
                        string jsonData2 = JsonConvert.SerializeObject(jsonRead);
                        System.IO.File.WriteAllText("Models/blogs.json",jsonData2);
                        return BlogMain(userid3);
                        
                    }
                    else{
                        control = true;
                    }
                }
            }
        }

        catch{
            ViewBag.Error = "There is no blog such as "+blogselect;
            return View("Error");
        }

        if(control){
            ViewBag.Error = "There is no blog such as "+blogselect;
            return View("Error");
        }
        else{
            return BlogMain(userid3);
        }
        
    }


    //blog editing window.
    public IActionResult BlogEdit(string userid2,string blogselect)
    {

        ViewBag.user = userid2;
        bool control = false;

        try{

            if(string.IsNullOrEmpty(blogselect)){
                ViewBag.Error = "Please select one of the blog!";
                return View("Error");
            }

            else{
                string jsonData = System.IO.File.ReadAllText("Models/blogs.json");
                List<Blog> jsonRead = JsonConvert.DeserializeObject<List<Blog>>(jsonData);
                
                //getting contents of the selected blog to edit it in the next window.
                foreach(var item in jsonRead){

                    if("BLOG ID: " +item.blogId + ">>>> BLOG TITLE: " +item.title + ">>>> OWNER ID: " + item.ownerId + ">>>> PUBLISH DATE: " + item.datetime == blogselect){
                        ViewBag.editOwnerId = item.ownerId;
                        ViewBag.editBlogId = item.blogId;
                        ViewBag.editTitle = item.title;
                        ViewBag.editContent = item.content;

                        return View("BlogEdit");                        
                    }
                    else{
                        control = true;
                    }
                }
            }
        }
        catch{
            ViewBag.Error = "There is no blog such as "+blogselect;
            return View("Error");
        }

        if(control){
            ViewBag.Error = "There is no blog such as "+blogselect;
            return View("Error");
        }
        else{
            return View("BlogEdit");
        }
    }


    //editing selected blog (the last edit button).
    public IActionResult BlogEditList(string ownerid2, string edit_title, string edit_content){

        //sepearting owner and blog id information which came from html view.
        string[] words = ownerid2.Split(',');
        string owner = words[0];
        string blog_id = words[1];

        if(string.IsNullOrEmpty(edit_title) || string.IsNullOrEmpty(edit_content)){
            ViewBag.Error = "Title or Content cannot be empty. Please fill them.";
            return View("Error");
        }

        List<Blog> blogJson = new List<Blog>();
        
        try{

            //json
            string jsonData2 = System.IO.File.ReadAllText("Models/blogs.json");
            List<Blog> jsonRead = JsonConvert.DeserializeObject<List<Blog>>(jsonData2);

            //changing new properties of selected blog.
            foreach(var item in jsonRead){

                if(blog_id == item.blogId){

                    item.title = edit_title;
                    item.content = edit_content;
                    item.datetime = DateTime.Now;

                    string jsonData = JsonConvert.SerializeObject(jsonRead);
                    System.IO.File.WriteAllText("Models/blogs.json",jsonData);
                    return BlogMain(owner);
                }
            }
        }
        catch{
        }
        ViewBag.Error = "Some error has occured!";
        return View("Error");
        
    }

    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
