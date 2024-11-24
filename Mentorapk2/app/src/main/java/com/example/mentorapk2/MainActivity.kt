package com.example.mentorapk2

import android.os.Bundle
import androidx.activity.ComponentActivity
import androidx.activity.compose.setContent
import androidx.compose.foundation.Image
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.*
import androidx.compose.foundation.lazy.LazyColumn
import androidx.compose.foundation.text.BasicTextField
import androidx.compose.material.icons.Icons
import androidx.compose.material.icons.filled.Add
import androidx.compose.material.icons.filled.ArrowBack
import androidx.compose.material.icons.filled.Check
import androidx.compose.material.icons.filled.Home
import androidx.compose.material3.*
import androidx.compose.runtime.*
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.res.painterResource
import androidx.compose.ui.tooling.preview.Preview
import androidx.compose.ui.unit.dp
import androidx.navigation.NavController
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.example.mentorapk2.ui.theme.Mentorapk2Theme
import retrofit2.Call
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST
import retrofit2.http.Path

// Модель данных для пользователя.
data class User(val id: Int? = null, val name: String, val surname: String, val secname: String, val age: Int, val specialty: String, val university: String)
data class RegisterRequest(val name: String, val age: Int, val specialty: String)
data class Post(val question: String, val tags: List<String>, val answers: MutableList<Answer> = mutableListOf())
data class Answer(val text: String)

// Интерфейс API для Retrofit.
interface ApiService {
    @GET("user")
    fun getUser(): Call<List<User>>

    @POST("register")
    fun registerUser(@Body user: RegisterRequest): Call<User>

    @GET("posts")
    fun getPosts(): Call<List<Post>>

    @POST("posts")
    fun createPost(@Body post: Post): Call<Post>

    @GET("posts/{id}/answers")
    fun getAnswers(@Path("id") postId: Int): Call<List<Answer>>

    @POST("posts/{id}/answers")
    fun addAnswer(@Path("id") postId: Int, @Body answer: Answer): Call<Answer>
}

class MainActivity : ComponentActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContent {
            Mentorapk2Theme {
                Surface(modifier = Modifier.fillMaxSize()) {
                    RegistrationScreen()
                }
            }
        }
    }
}

@Composable
fun RegistrationScreen() {
    val navController = rememberNavController()

    NavHost(navController = navController, startDestination = "login") {
        composable("login") { LoginScreen(navController) }
        composable("register") { RegisterScreen(navController) }
        composable("question_answer") { QuestionAnswerScreen(navController) }
        composable("profile") { ProfileScreen(navController) }
        composable("discussion/{question}/{answers}") { backStackEntry ->
            val question = backStackEntry.arguments?.getString("question")
            val answers = backStackEntry.arguments?.getString("answers")
                ?.split(",")?.map { Answer(it) }?.toMutableList() ?: mutableListOf()
            DiscussionScreen(question ?: "", answers, navController)
        }
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun LoginScreen(navController: NavController) {
    Column(
        modifier = Modifier.fillMaxSize().padding(16.dp),
        horizontalAlignment = Alignment.CenterHorizontally,
        verticalArrangement = Arrangement.Center
    ) {
        Text("Вход", style = MaterialTheme.typography.headlineMedium)

        Spacer(modifier = Modifier.height(32.dp))

        var login by remember { mutableStateOf("") }
        var password by remember { mutableStateOf("") }

        BasicTextField(
            value = login,
            onValueChange = { login = it },
            modifier = Modifier.fillMaxWidth().padding(8.dp),
            decorationBox = { innerTextField ->
                if (login.isEmpty()) {
                    Text("Введите логин")
                }
                innerTextField()
            }
        )

        BasicTextField(
            value = password,
            onValueChange = { password = it },
            modifier = Modifier.fillMaxWidth().padding(8.dp),
            decorationBox = { innerTextField ->
                if (password.isEmpty()) {
                    Text("Введите пароль")
                }
                innerTextField()
            }
        )

        Button(onClick = {
            // Переход на главный экран (вопросы и ответы)
            navController.navigate("question_answer")
        }) {
            Text("Войти")
        }

        Spacer(modifier = Modifier.height(16.dp))

        Text(
            text = "Нет аккаунта? Зарегистрируйтесь",
            color = Color.Blue,
            modifier = Modifier.clickable { navController.navigate("register") }
        )
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun RegisterScreen(navController: NavController) {
    var name by remember { mutableStateOf("") }
    var age by remember { mutableStateOf("") }
    var specialty by remember { mutableStateOf("") }
    var isLoading by remember { mutableStateOf(false) }
    var errorMessage by remember { mutableStateOf<String?>(null) }

    // Инициализация Retrofit для получения данных о пользователе.
    val retrofit = Retrofit.Builder()
        .baseUrl("https://your-api-url.com/") // Замените на URL вашего API.
        .addConverterFactory(GsonConverterFactory.create())
        .build()

    val apiService = retrofit.create(ApiService::class.java)

    Column(
        modifier = Modifier.fillMaxSize().padding(16.dp),
        horizontalAlignment = Alignment.CenterHorizontally,
        verticalArrangement = Arrangement.Center
    ) {
        Text("Регистрация", style = MaterialTheme.typography.headlineMedium)

        Spacer(modifier = Modifier.height(32.dp))

        BasicTextField(
            value = name,
            onValueChange = { name = it },
            modifier = Modifier.fillMaxWidth().padding(8.dp),
            decorationBox = { innerTextField ->
                if (name.isEmpty()) {
                    Text("Введите имя")
                }
                innerTextField()
            }
        )

        BasicTextField(
            value = age,
            onValueChange = { age = it },
            modifier = Modifier.fillMaxWidth().padding(8.dp),
            decorationBox = { innerTextField ->
                if (age.isEmpty()) {
                    Text("Введите курс")
                }
                innerTextField()
            }
        )

        BasicTextField(
            value = specialty,
            onValueChange = { specialty = it },
            modifier = Modifier.fillMaxWidth().padding(8.dp),
            decorationBox = { innerTextField ->
                if (specialty.isEmpty()) {
                    Text("Введите специальность")
                }
                innerTextField()
            }

        )

        Button(onClick = {
            if (name.isNotBlank() && age.isNotBlank() && specialty.isNotBlank()) {
                isLoading = true

                // Создание объекта пользователя для отправки на сервер.
                val userToRegister = RegisterRequest(name, age.toInt(), specialty)

                apiService.registerUser(userToRegister).enqueue(object : retrofit2.Callback<User> {
                    override fun onResponse(call: Call<User>, response: retrofit2.Response<User>) {
                        isLoading = false

                        if (response.isSuccessful) {
                            // Успешная регистрация, можно перейти на экран профиля или входа.
                            navController.navigate("login")
                        } else {
                            errorMessage = "Ошибка регистрации"
                        }
                    }

                    override fun onFailure(call: Call<User>, t: Throwable) {
                        isLoading = false
                        errorMessage = "Ошибка сети: ${t.message}"
                    }
                })
            } else {
                errorMessage = "Заполните все поля"
            }
        }) {
            Text("Зарегистрироваться")
        }

        Spacer(modifier=Modifier.height(16.dp))

        errorMessage?.let { message ->
            Text(text=message, color=Color.Red)
        }

        Spacer(modifier=Modifier.height(16.dp))

        Text(
            text="Уже есть аккаунт? Войдите",
            color=Color.Blue,
            modifier=Modifier.clickable { navController.navigate("login") }
        )
    }
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun QuestionAnswerScreen(navController: NavController) {
    var question by remember { mutableStateOf("") }
    var tags by remember { mutableStateOf("") }
    val questionsAndAnswers = remember { mutableStateListOf<Post>() }

    // Логика создания поста (вопроса)
    var isCreatingPost by remember { mutableStateOf(false) }

    Scaffold(
        topBar={
            TopAppBar(
                title={ Text ("Вопросы и Ответы")},
                navigationIcon={
                    IconButton(onClick={ navController.popBackStack() }){
                        Icon(imageVector=Icons.Default.ArrowBack, contentDescription="Назад")
                    }
                },
                actions={
                    if(isCreatingPost){
                        IconButton(onClick={
                            if(question.isNotBlank() && tags.isNotBlank()){
                                questionsAndAnswers.add(Post(question, tags.split(",").map{ it.trim() }.toList()))
                                question=""
                                tags=""
                                isCreatingPost=false
                            }
                        }){
                            Icon(imageVector=Icons.Filled.Check, contentDescription="Создать пост")
                        }
                    } else{
                        IconButton(onClick={ isCreatingPost=true }){
                            Icon(imageVector=Icons.Filled.Add, contentDescription="Создать пост")
                        }
                    }

                    IconButton(onClick={ navController.navigate ("profile") }){
                        Icon(imageVector=Icons.Filled.Home, contentDescription="Профиль студента", modifier=Modifier.size(24.dp))
                    }
                },
            )
        },
        content={ padding ->
            Column(
                modifier=Modifier.fillMaxSize().padding(padding).padding(16.dp),
                verticalArrangement=Arrangement.spacedBy(16.dp)

            ) {

                if(isCreatingPost){

                    BasicTextField(value=question,
                        onValueChange={ question=it },
                        modifier=Modifier.fillMaxWidth(),
                        decorationBox={innerTextField ->
                            if(question.isEmpty()){
                                Text ("Введите ваш вопрос")
                            }
                            innerTextField()
                        })

                    BasicTextField(value=tags,
                        onValueChange={ tags=it },
                        modifier=Modifier.fillMaxWidth(),
                        decorationBox={innerTextField ->
                            if(tags.isEmpty()){
                                Text ("Введите теги (через запятую)")
                            }
                            innerTextField()
                        })
                } else{

                    LazyColumn(modifier=Modifier.fillMaxSize()) {

                        items(questionsAndAnswers.size) { index ->

                            val post=questionsAndAnswers[index]
                            Card(
                                modifier=Modifier.fillMaxWidth().padding(vertical=8.dp)
                                    .clickable(onClick={
                                        val answersString=post.answers.joinToString(","){ it.text}
                                        navController.navigate ("discussion/${post.question}/$answersString")
                                    }),
                                elevation=CardDefaults.cardElevation(defaultElevation=4.dp),
                                colors=CardDefaults.cardColors(containerColor= Color(0xFFE0F7FA))
                            ) {

                                Column(modifier=Modifier.padding(16.dp)) {

                                    Text(text="Вопрос:${post.question}", style=MaterialTheme.typography.bodyMedium)
                                    Text(text="Теги:${post.tags.joinToString(", ")}", style=MaterialTheme.typography.bodySmall)

                                    Spacer(modifier=Modifier.height(8.dp))

                                    post.answers.forEach{ answer ->
                                        Text(text="Ответ:${answer.text}", style=MaterialTheme.typography.bodySmall)
                                    }

                                    Spacer(modifier=Modifier.height(8.dp))

                                    Button(onClick={
                                        val answersString=post.answers.joinToString(","){ it.text}
                                        navController.navigate ("discussion/${post.question}/$answersString")
                                    }){
                                        Text ("Ответить")
                                    }

                                }

                            }

                        }

                    }

                }

            }

        })

}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun DiscussionScreen(question:String, existingAnswers: List<Answer>, navController: NavController) {

    var newAnswer by remember{ mutableStateOf("")}
    val answersList by remember{ mutableStateOf(existingAnswers.toMutableList()) }

    Scaffold(
        topBar={
            TopAppBar(
                title={ Text ("Обсуждение вопроса")},
                navigationIcon={
                    IconButton(onClick={ navController.navigate ("question_answer")}){
                        Icon(imageVector=Icons.Default.ArrowBack, contentDescription="Назад")
                    }
                })
        },
        content={ padding ->

            Column(
                modifier=Modifier.padding(padding).padding(16.dp),
                verticalArrangement=Arrangement.spacedBy(8.dp),
                horizontalAlignment=Alignment.Start,
            ) {

                Text(text="Вопрос:$question", style=MaterialTheme.typography.headlineMedium)
                Spacer(modifier=Modifier.height(16.dp))

                BasicTextField(value=newAnswer,
                    onValueChange={ newAnswer=newAnswer},
                    modifier=Modifier.fillMaxWidth(),
                    decorationBox={innerTextField ->
                        if(newAnswer.isEmpty()){
                            Text ("Введите ваш ответ", style=MaterialTheme.typography.bodySmall)
                        }
                        innerTextField()
                    })

                Button(onClick={
                    if(newAnswer.isNotBlank()){
                        answersList.add(Answer(newAnswer))
                        newAnswer=""
                    }}){
                    Text ("Отправить ответ") }

                Spacer(modifier=Modifier.height(16.dp))

                answersList.forEach{ answer ->
                    Text(text="Ответ:${answer.text}", style=MaterialTheme.typography.bodySmall)
                }
            }})
}

@OptIn(ExperimentalMaterial3Api::class)
@Composable
fun ProfileScreen(navController: NavController) {

    var user by remember{ mutableStateOf<User?>(null)}
    var isLoading by remember{ mutableStateOf(true)} // Состояние загрузки данных о пользователе.
    var errorMessage by remember{ mutableStateOf<String?>(null)} // Сообщение об ошибке.

    // Инициализация Retrofit для получения данных о пользователе.
    val retrofit  =
        Retrofit.Builder()
            .baseUrl ("https://bmu7ubnrybuih9h0cvz8-mysql.services.clever-cloud.com") // Замените на URL вашего сервера.
            .addConverterFactory(GsonConverterFactory.create())
            .build()

    val apiService  =
        retrofit.create(ApiService::class.java)

    LaunchedEffect(Unit){
        apiService.getUser().enqueue(object : retrofit2.Callback<List<User>>{
            override fun onResponse(call: Call<List<User>>, response: retrofit2.Response<List<User>>){
                if(response.isSuccessful){
                    user=response.body()?.firstOrNull() // Получаем первого пользователя из списка.
                } else{
                    errorMessage="Ошибка загрузки данных"
                }

                isLoading=false // Устанавливаем состояние загрузки в false после ответа.
            }

            override fun onFailure(call: Call<List<User>>, t: Throwable){
                errorMessage="Ошибка сети:${t.message}"
                isLoading=false // Устанавливаем состояние загрузки в false при ошибке.
            }

        })
    }

    Column (
        modifier  =
        Modifier.fillMaxSize(),
        horizontalAlignment  =
        Alignment.CenterHorizontally,
        verticalArrangement  =
        Arrangement.Top){

        TopAppBar (
            title  =
            { Text ("Профиль студента")},
            navigationIcon  =
            { IconButton(onClick={navController.navigate ("question_answer")}){ Icon(imageVector=
            Icons.Default.ArrowBack, contentDescription="Назад")}
            })

        Spacer(modifier  =
        Modifier.height (16.dp))

        when{
            isLoading-> Text(text="Загрузка...")
            errorMessage!=null-> Text(text=
            errorMessage!! , color=
            Color.Red)

            user!=null->{
                Text(text=
                "Имя:${user!!.name}")
                Text(text=
                "Возраст:${user!!.age}лет ")
                Text(text=
                "Специальность:${user!!.specialty}")
            } user == null -> {
            Text(text="Пользователь не найден.")
        }

        }

        Spacer(modifier  =
        Modifier.height (16.dp))

        Image (
            painter=painterResource(id=
            R.drawable.student_photo),
            contentDescription="Фотография студента",
            modifier  =
            Modifier.size (128.dp),)

        Spacer(modifier  =
        Modifier.height (16.dp))

        // Пример статической информации о студенте:
        Text(text=
        "Имя:Ivan Ivanov ")
        Text(text=
        "Возраст :20 лет ")
        Text(text=
        "Специальность :Программная инженерия ")
    }

}

@Preview(showBackground=true)
@Composable
fun PreviewProfileScreen() {

    Mentorapk2Theme{
        ProfileScreen (rememberNavController ())
    }

}
