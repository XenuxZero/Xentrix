// LIBRERÍAS DE UNITY QUE SE UTILIZARÁN

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/* HE OPTADO POR UNA SOLA CLASE EN UN ÚNICO FICHERO, PERO SE PUEDE PARTIR EN SUS DIVERSAS FUNCIONALIDADES. EN FICHEROS
DIFERENTES. PLAYGROUND O ZONA DE JUEGO Y CONTIENE TODA LA LÓGICA DE XENTRIX(tm) */

public class Playground : MonoBehaviour //NUEVA CLASE "Playground" HIJA DE LA CLASE ESTANDAR DE UNITY "MonoBehaviour".
{
    /* ------------------- VARIABLES ------------------- */

    //VARIABLES CONSTANTES ENTERAS DE SOLO LECTURA, NO PUEDEN CAMBIAR. DEFINO LAS PUNTUACIONES DE RECOMPENSA.
    const short BLOCKVALUE = 100; //100 PUNTOS CADA VEZ QUE COLOCO UN BLOQUE EN UN SITIO.
    const int LINEVALUE = 1000; //1000 PUNTOS POR CADA LÍNEA.

    const int LEVELVALUE = 10000; //AQUÍ DEFINO LA PUNTUACIÓN NECESARIA PARA PASAR DE NIVEL.

    private short ancho = 10; // DEFINO UN SHORT (VALORES ENTRE 0-255 SIN SIGNO, OCUPA MENOS MEMORIA QUE UN INT) PARA EL ANCHO DEL TABLERO.
    private short alto = 24; /* LO MISMO PERO PARA EL ALTO. LAS VARIABLES PÚBLICAS SE PUEDEN VER EN EL INSPECTOR DE UNITY!
                               PUEDES MODIFICARLAS DENTRO DE UNITY O HACER REFERENCIAS CON UN SIMPLE DRAG AND DROP! */

    [Header("Contadores")] // ESTO SIRVE PARA EL INSPECTOR DE UNITY, DIVIDE BLOQUES DE VARIABLES PARA ORDENARLAS.
    public Text score; // HACEMOS PÚBLICA PARA REFERENCIAR EL CAMPO DE TEXTO DE LA PUNTUACIÓN EN UNA VARIABLE TIPO TEXT.
    public Text lvlUnidades; // LO MISMO PERO PARA LAS UNIDADES DEL TEXTO DEL NIVEL.
    public Text lvlDecenas; // LAS DECENAS, LOS SEPARÉ PORQUE VISUALMENTE SE DESCUADRABAN LOS DÍGITOS AL CAMBIAR DE NÚMEROS. TOC.
    private short unidades; // LAS PRIVADAS SOLO SON ACCESIBLES AQUÍ Y EN ESTA CLASE. UN SHORT PARA LAS UNIDADES.
    private short decenas; // SHORT PARA LAS DECENAS. CON ESTAS MANEJAREMOS LA LÓGICA, CON LOS TEXTs A NIVEL GRÁFICO.
    private short currentLevel; // AQUÍ SE GUARDARÁ EL NIVEL ACTUAL. A TIEMPO DE REAL.
    private short maxLevel; // DEFINIREMOS EL MÁXIMO NIVEL POSIBLE EN EL JUEGO.
    private int scoreValue; // IRÁ LA PUNTUACIÓN A TIEMPO REAL. UN ENTERO, PUES EL SHORT SE QUEDARÁ IDEM... JE!
    private int totalLines; // LA CANTIDAD DE LÍNEAS HECHAS POR EL JUGADOR. UN ENTERO, POR SI ACASO EL JUGADOR ES UN HACHA.
    private int nTimesLevelScore; // CUENTA LAS VECES QUE PASAMOS DE PUNTUACIÓN (10000) COMO PARA UNA SUBIDA DE NIVEL.

    [Header("Botones")]
    public GameObject resume; // REFERENCIA GRÁFICA PARA EL BLOQUE DE PAUSA Y REANUDAR JUEGO.
    public GameObject gameOver; // REFERENCIA GRÁFICA PARA EL BLOQUE DE GAME OVER Y REINICIAR.
    public GameObject quitGame; // REFERENCIA GRÁFICA PARA EL BOTÓN QUITAR JUEGO TRAS PAUSAR O GAME OVER.
    public Button left, right, down, drop, rotate, pause; /* REFERENCIAS GRÁFICAS A LOS BOTONES DE DIRECCIÓN.
                                                               IZQUIERDA, DERECHA, ABAJO, AL FONDO, ROTAR Y PAUSA
                                                               PUEDES DEFINIR VARIAS CON UNA COMA EN LUGAR DE LÍNEA
                                                               POR LÍNEA COMO LAS ANTERIORES */

    public Image un_Mute_img; /* REFERENCIA AL COMPONENTE "Image" DEL BOTÓN MUTE DEL MENÚ DE PAUSA. ASÍ PODRÉ ACCEDER
                                 A LA PROPIEDAD "Sprite" Y CAMBIAR LAS IMÁGENES DINÁMICAMENTE. */
    

    [Header("Tetriminos")]
    public Transform[] prefabs; /* UNA MATRIZ DE TRANSFORMS. EN UNITY SE PUEDE HACER REFERENCIA A UN OBJETO CON "GameObject" o
                                   Transform. EL PRIMERO LO REFERENCIA BAJO EL PUNTO DE VISTA DE OBJETO, EL SEGUNDO LO
                                   REFERENCIA BAJO EL PUNTO DE VISTA DE SU POSICIÓN EN EL ESPACIO... ES COMO LLAMAR LA ATENCIÓN
                                   A UNA VIEJA QUE SE CUELA EN LA COLA DEL SUPERMERCADO, SE LE PUEDE LLAMAR VIEJA DE MIERDA O
                                   LA QUE SE HA PUESTO DELANTE DE TÍ XDXD, AMBAS DEFINICIONES HACEN REFERNCIA A LA MISMA PUERCA.
                                   EN ESTE CASO, EN ESTA MATRIZ SE AÑADIRÁN LOS DIFERENTES BLOQUES (TETRIMINOS) DEL JUEGO */

    public bool isConsecutiveSpawn; /* BOOLEANO PARA IDENTIFICAR SI EL JUEGO SPAWNEARÁ EL MISMO TETRIMINO DE FORMA CONSECUTIVA O
                                       SE ACTIVARÁ EL MÉTODO QUE LO PREVIENE Y HACE EL JUEGO ALGO MÁS DIFÍCIL. */

    private GameObject[,] playground; // ESTA MATRIZ BIDIMENSIONAL CONSTRUYE EL TABLERO DONDE CAERÁN LOS TETRIMINOS.
    private Transform newTetrimino; // AQUÍ GUARDAMOS EL BLOQUE QUE EL JUGADOR ESTÁ MOVIENDO ACTUALMENTE, UNO CADA VEZ.
    private Transform tetriminoHelper; /* COMO EL ANTERIOR, AQUÍ SE GUARDARÁ UNA COPIA DE "newTetrimino" PARA HACER EL AYUDANTE
                                          VISUAL. O LA SOMBRA... NO SÉ, LA PIEZA QUE MARCA LA CAÍDA. */
    private Transform[] minos; // EN ESTA MATRIZ GUARDAMOS CADA PIEZA (MINOS) QUE COMPONE EL TETRIMINO QUE SE ESTÁ JUGANDO.
    private Transform[] minosHelper; /* COPIA DE MINOS, PERO PARA EL AYUDANTE DE CAÍDA.*/

    // ESTAS VARIABLES LAS USO PARA CONSTRUIR EL RELOJ INTERNO DEL JUEGO.
    private float timerLimit; // UN FLOAT (DECIMAL) PARA DEFINIR LOS SEGUNDOS MÁXIMOS PARA CONSIDERAR UN TIC DEL RELOJ.
    private float elapsedTime; // AQUÍ GUARDO LOS MILISEGUNDOS QUE HAN PASADO EN EJECUCIÓN.
    private float speedUpTime; /* AQUÍ DEFINO LOS MILISEGUNDOS QUE DEBE REDUCIR timerLimit PARA ACORTAR LOS TICS DEL RELOJ
                                  CADA VEZ QUE SUBAS DE NIVEL Y ASÍ HACER EL JUEGO MÁS RÁPIDO. */

    private short nBlocks; // NÚMERO DE BLOQUES QUE SE USARÁN EN XENTRIX. MODIFICABLE AÑADIENDO NUEVOS.

    private bool isGameOver; // ESTÁ EL JUEGO EN MODO GAME OVER?
    private bool isPaused; // ESTÁ PAUSADO?
    private bool isMute; // ESTÁ MUTEADA LA MÚSICA?

    private float lineSpeed; // AQUÍ DEFINO LA VELOCIDAD CON LA QUE LAS LÍNEAS SE ELIMINAN O CAMBIAN DE COLOR (A BLANCO).

    // OTRO RELOJ, PERO ESTA VEZ CALCULA EL TIEMPO DE BONUS (2s) QUE TE DOY PARA QUE SUMES EL MULTIPLICADOR.
    private short bonusTimerLimit; // LÍMITE DE TIC. 2 SEGUNDOS.
    private short bonusMultiplier; // VALOR DE MULTIPLICADOR DE PUNTUACIÓN, A PARTIR DEL x2 SE CUENTA YA QUE: 1000 x x1 = 1000.

    private bool isBonusTime; // ESTÁ EL JUEGO EN MODO BONO POR MULTILÍNEA? Y AQUÍ EMPEZAMOS A CONTAR LOS 2 SEGUNDOS.

    private float elapsedBonusTime; // EL TIEMPO QUE HA TRANSCURRIDO EN EJECUCIÓN.
    private short bonusFontSize; /* AQUÍ CAMBIO EL TAMAÑO DE LA FUENTE DEL TESTIGO GRÁFICO PARA HACERLO MÁS GRANDE A CUANTA
                                  MÁS LÍNEAS HAYAS HECHO. */

    public Text multiplier; // REFERENCIA AL TESTIGO GRÁFICO DEL MULTIPLICADOR.
    private Vector3 vectorOrigin; /* ESTO ES UN VECTOR DE POSICIÓN DE ORIGEN PARA EL TESTIGO GRÁFICO DEL MULTIPLICADOR.
                                     BÁSICAMENTE SIRVE PARA MANTENER EL TESTIGO EN EL CENTRO CUANDO LE DE EL TEMBLEQUE Y
                                     NO SE VAYA FUERA DEL ESCENARIO COMO HA HECHO OTRAS VECES...*/

    private int blockSelected; // AQUÍ SE GUARDA EL TETRIMINO SELECCIONADO.

    [Header("Sprites Estéticos")]
    public Sprite tetrimino_graph; /* REFERENCIA AL SPRITE QUE HACE DE TEXTURA PARA LOS "LADRILLOS" DE LOS TETRIMINOS. */
    public Sprite unMute_graph; /* REFERENCIA AL SPRITE DE UNMUTE PARA EL BOTÓN DE SILENCIAR MÚSICA. */
    public Sprite mute_graph; /* LO MISMO PERO PARA MUTE */

    public ParticleSystem sprinkles; /* ESTO ES UN SISTEMA DE PARTÍCULAS. BÁSICAMENTE UN GENERADOR DE DE PARTÍCULAS QUE
                                        PUEDES CONFIGURAR A TU GUSTO Y DE PE A PA. PUEDE USARSE TANTO CON 2D COMO 3D.
                                        EN XENTRIX SIMULA CHISPAS INCANDESCENTES SALIENDO DEL TESTIGO DEL MULTIPLICADOR
                                        CUANDO EL USUARIO SOBREPASA x4 QUE EQUIVALE A UN MULTIPLICADOR QUE TE DA UNA "I".
                                        A VER SI CONSIGUES VERLOS!.
                                        A VECES A UNAS MECÁNICAS ABURRIDAS, ES BUENO AÑADIR DETALLES INESPERADOS PARA
                                        DESPERTAR EL INTERÉS DEL JUGADOR A SEGUIR INTENTANDO VER QUE MÁS HAY ALLÁ.
                                        (NO HAY MÁS xdxdxd) */

    [Header("Audios")] /* SECCIÓN DE AUDIO */

    public AudioSource[] SoundFX; /* ESTO ES UNA MATRIZ DE REFERENCIAS A 3 COMPONENTES "AudioSource" QUE ESTÁN EN "Grid"
                                     SE PUEDEN PONER MÁS DE UN MISMO TIPO DE COMPONENTE EN UN GameObject, PERO LUEGO LES
                                     TIENES QUE HACER REFERENCIAS PARA MANEJARLOS AQUÍ. CADA AudioSource O FUENTE DE SONIDO
                                     SERÍA ALGO ASÍ COMO... QUE TODO OBJETO QUE TENGA ESTO COMO COMPONENTE, EMITE SONIDO.
                                     EN UN JUEGO COMO ESTE NO TIENE MUCHO MÉRITO, PERO EN 3D, PUEDE SER UN PÁJARO POR
                                     EJEMPLO, AudioSource TIENE UNA ESFERA DE ALCANCE, SI LAS OREJAS DEL JUGADOR NO ESTÁN
                                     DENTRO DE SU ESFERA DE ALCANCE NO LO OIRÁS. TAMBIÉN ES CAPAZ DE IMITAR EL EFECTO 
                                     DOPPLER, QUE ES EL CAMBIO DE FRECUENCIA DE UN SONIDO CON RESPECTO A LA DISTANCIA
                                     DEL OYENTE... YA PODEMOS HACER SIRENAS DE AMBULANCIA.
                                     ESTA MATRIZ SE ENCARGA DE GUARDAR LOS EFECTOS DE SONIDO.*/

    public AudioSource asMusic; /* REFERENCIA AL EMISOR DE LA MÚSICA, QUE ES LA PROPIA "MAINCAMERA" DE XENTRIX. */

    private bool isJoyAxisXPressed, isJoyAxisYPressed; /* DOS VARIABLES BOOL PARA DETERMINAR SI LA CRUCETA DEL MANDO ESTÁ
                                                          EN UN EJE DETERMINADO. EL DRIVER DE MI MANDO EMULA AL DE UNA XBOX.
                                                          POR LO VISTO LA CRUCETA LA TOMA COMO UN JOYSTICK, ASÍ QUE DEVUELVE
                                                          VALORES DE (-1 <== 0 ==> 1). Y COMO SE EJECUTARÁ EN UN UPDATE, SI
                                                          DEJAS PULSADO LA CRUCETA PARA UN LADO (1 0 -1) LOS TETRIMINOS SE
                                                          MOVERÁN A SUPERVELOCIDAD PORQUE LO TOMA COMO SI PULSARAS REPETIDAMENTE
                                                          PARA UN LADO. ASÍ QUE ESTAS VARIABLES SERÁN SU LIMITADOR DE PULSACIÓN.*/

    private bool isTactile; /* DETERMINAREMOS SI XENTRIX SE EJECUTA EN UN DISPOSITIVO TACTIL (MOVIL) O NO (PC) */

    /* ------------------- FUNCIONES DE EJECUCIÓN O MÉTODOS DE CLASE HEREDADAS DE MONOBEHAVIOR ------------------- */

    /* A DIFERENCIA DE Start(), Awake() SE EJECUTA ANTES QUE ESTA Y ES IDEAL PARA INICIALIZAR VARIABLES TOCHAS O HACER
     * REFERENCIAS COSTOSAS. SE EJECUTA NADA MÁS INICIAR LA APP. QUE COMIENCE LA DEFINICIÓN DE VARIABLES! */

    private void Awake()
    {
        if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer)
            /* ESTO ES NUEVO, COMPROBAMOS ANTES QUE NADA SI XENTRIX SE EJECUTA EN PC, O POR EL CONTRARIO EN ANDROID O IPHONE (TACTILES)*/
        {
            HidePhoneControls(); /* SI SE EJECUTA EN PC, DESACTIVA LOS CONTROLES DE MOVIL */
            isTactile = false; /* DETERMINA QUE ESTAMOS SOBRE UN DISPOSITIVO CON TECLADO O MANDO (PC) */
        }
        else
        {
            isTactile = true; /* SI NO, ESTAMOS EN UN DISPOSITIVO TÁCTIL ASÍ PODREMOS DESACTIVAR LAS COMPROBACIONES DE PULSADO
                                 DE TECLAS (KEYLISTENERS) Y AHORRAR CICLOS.*/
        }

        Application.targetFrameRate = -1; /* ESTO AJUSTA LOS FPS MÁXIMOS AL ESTÁNDAR NATIVO DEL DISPOSITIVO */

        playground = new GameObject[ancho, alto]; // CONSTRUYO EL TABLERO CON LAS VARIABLES DE ALTO Y ANCHO.
        maxLevel = 99; // DEFINO EL MÁXIMO NIVEL ALCANZABLE, 99 INHUMANOS NIVELES.
        currentLevel = 0; // NIVEL ACTUAL 0, OBVIAMENTE.
        unidades = 0; // TESTIGOS GRÁFICOS DE LOS NIVELES A 0 EN UNIDADES.
        decenas = 0; // A CERO EN DECENAS.
        timerLimit = 1f; // LÍMITE DE TIC DE RELOJ A UN SEGUNDO. POR CIERTO, LOS VALORES DECIMALES SE REPRESENTAN CON UNA F. 1f = 1.0
        elapsedTime = 0f; // TIEMPO TRANSCURRIDO A 0.0.
        speedUpTime = 0.025f; // TIEMPO DE ACORTE DE TIC DE RELOJ PARA IR MÁS RÁPIDO CADA VEZ QUE PASAS DE NIVEL.
        scoreValue = 0; // VALOR DE PUNTUACIÓN A 0.
        totalLines = 0; // LÍNEAS TOTALES A 0.
        nTimesLevelScore = 0; // 0 VECES PASAMOS LA PUNTUACIÓN DE 10000.
        nBlocks = (short) prefabs.Length; // NÚMERO DE TETRIMINOS QUE ENTRARÁN EN JUEGO, ASÍ AÑADÍ EL PEQUEÑO. PUEDES AÑADIR LOS QUE QUIERAS.
        newTetrimino = null; // INICIALIZO LA PIEZA JUGABLE A NULO.
        tetriminoHelper = null; /* INICIALIZO EL AYUDANTE A NULO. */
        isPaused = true; // EL JUEGO COMIENZA PAUSADO ASÍ QUE INICIALIZO A TRUE.
        isGameOver = false; // SI ES GAME OVER A FALSO.
        lineSpeed = 0.05f; // ESTABLEZCO LA VELOCIDAD DE ROTURA DE LÍNEA. LO SUFICIENTE PARA QUE VEAS COMO CAMBIA DE COLOR.
        bonusMultiplier = 0; // MULTIPLICADOR A 0.
        isBonusTime = false; // INICIAMOS A FALSO EL TIEMPO DE BONO POR MULTILÍNEA.
        elapsedBonusTime = 0f; // A 0 EL TIEMPO TRANSCURRIDO EN EJECUCIÓN.
        bonusTimerLimit = 2; // LOS SEGUNDOS MÁXIMOS DE TIEMPO DE BONUS.
        blockSelected = -1; /* INICIALIZO EL TETRIMINO SELECCIONADO A -1 */
        vectorOrigin = new Vector3(multiplier.transform.position.x, multiplier.transform.position.y, 0f); /* ESTABLEZCO LA POSICIÓN ORIGEN
                                                                                                             DEL TESTIGO DEL MULTIPLICADOR
                                                                                                             PARA QUE NO SE MUEVA DEL SITIO
                                                                                                             CUANDO SE PONGA A VIBRAR. */


        score.text = scoreValue.ToString(); /* ESTABLEZCO EL TEXTO DE LA PUNTUACIÓN CON EL VALOR DE LA PUNTUACIÓN. .ToString()
                                               SIRVE COMO CONVERSIÓN EXPLÍCITA DE UN VALOR NUMÉRICO A CADENA, PUESTO QUE .text
                                               SOLO ADMITE STRINGS */

        lvlDecenas.text = decenas.ToString(); // ESTABLEZCO EL DÍGITO DE LAS DECENAS AL VALOR DE LAS DECENAS.
        lvlUnidades.text = unidades.ToString(); // ESTABLEZCO EL DÍGITO DE LAS UNIDADES AL VALOR DE LAS UNIDADES.
        multiplier.gameObject.SetActive(false); // MIENTRAS QUE NO ESTÉ EN MODO BONUS, QUE NO APAREZCA EL TESTIGO DE MULTIPLICADOR.
        bonusFontSize = 10; // ESTABLECER EL TAMAÑO DEL TEXTO DEL TESTIGO GRÁFICO DEL BONUS MULTIPLICADOR.
        isJoyAxisXPressed = false;
        isJoyAxisYPressed = false;

        gameOver.SetActive(isGameOver); // ACTIVA LA PANTALLA DE GAME OVER SEGÚN EL ESTADO isGameOver.    
        resume.SetActive(isPaused); // ACTIVA LA PANTALLA DE PAUSA SEGÚN EL ESTADO isPaused.
        ToggleButtons(!isPaused); // ESTA FUNCIÓN SE ENCARGA DE INHABILITAR LOS BOTONES DE JUEGO CUANDO HAY PANTALLAS DE PAUSA O GAMEOVER.

        ScoreUp(scoreValue); // ESTABLECE EL VALOR INICIAL DE LA PUNTUACIÓN.
        GeneratePlayground(); // ESTA FUNCIÓN GENERA EL TABLERO DE JUEGO.
        Spawner(); // DESPUÉS ESTA FUNCIÓN GENERA UN TETRIMINO ALEATORIO AL ESCENARIO.

        if(!isMute) /* SI NO ESTÁ MUTEADO */
        {
            asMusic.Play(); /* QUE COMIENCE A SONAR LA MÚSICA */
            asMusic.Pause(); /* PERO PÁUSAMELA, PORQUE QUIERO QUE SE OIGA NADA MÁS EL JUGADOR LE DE "A JUGAR".
                                ESTO ESTÁ AQUÍ PARA CUANDO EL JUGADOR ELIJA REINICIAR PARTIDA Y RESETEEN LOS VALORES, NO AFECTE AL SONIDO. */
        }
    }

    /* ESTA FUNCIÓN ES LA ESTANDAR DE UNITY JUNTO CON "Start()". SE EJECUTA CADA FRAME ASÍ QUE AQUÍ VIENE PRÁCTICAMENTE TODA
     * LA LÓGICA DE LA APP A EJECUTARSE. */
    private void Update()
    {
        if (!isGameOver && !isPaused && newTetrimino != null) // SI NO ES GAME OVER Y NO ESTÁ PAUSADO Y YA SE HA GENERADO UN TETRIMINO ALEATORIO.
        {
            asMusic.volume = 0.10f; /* AL BUEN "HARDCODING". CUANDO EL USUARIO EMPIECE LA PARTIDA, ESTABLECE UN VOLUMEN DE 0.10F.
                                       LA PISTA ESTÁ DEMASIADO ALTA Y LOS EFECTOS MUY BAJOS, SE PISAN. */
            asMusic.UnPause(); /* DESPAUSAR LA PISTA DE MÚSICA PARA QUE COMIENCE A SONAR! */

            tetriminoHelper.gameObject.SetActive(true); /* QUE EL AYUDANTE VISUAL DEL TETRIMINO SE VEA AHORA EN PANTALLA. */

            Timer(); // ...EMPIEZA A FUNCIONAR EL RELOJ INTERNO...

            Controls(); /* ACTIVO LOS CONTROLES DE PC Y MANDO... */

            HelperMimicking(); /* HAGO QUE EL AYUDANTE VISUAL IMITE COMO UN MIMO A LA PIEZA DEL JUGADOR... */

            if (isBonusTime) // Y SI ENTRAS EN TIEMPO DE BONUS...
            {
                if (bonusMultiplier > 4) /* SI EL MULTIPLICADOR ES MAYOR QUE x4... */
                {
                    multiplier.color = Color.Lerp(Color.white, Color.red, (bonusMultiplier - 4) / 5f); /* CAMBIA EL COLOR DEL TEXTO DEL
                                                                                                          MULTIPLICADOR PAULATINAMENTE
                                                                                                          DE BLANCO A ROJO, CUANTO MAYOR SEA
                                                                                                          EL BONUS MÁS ROJO SERÁ.
                                                                                                          "Color.Lerp" SE USA PARA HACER
                                                                                                          TRANSICIONES SUAVES DE ENTRE DOS
                                                                                                          VALORES, ALGO PARECIDO A LO QUE 
                                                                                                          HACÍA FLASH CON LAS INTERPOLACIONES
                                                                                                          PERO EN LUGAR DE ANIMACIÓN, VALORES.
                                                                                                          EN ESTE CASO "Lerp" ESTÁ ORIENTADO
                                                                                                          A VALORES DE COLOR, QUE SUELEN VALORES
                                                                                                          NORMALIZADOS, ES DECIR FLOATS DE 0 A 1
                                                                                                          EN LUGAR DE 0 A 255 COMO EN ADOBE
                                                                                                          PHOTOSHOP.*/

                    sprinkles.Play(); /* COMO SE SUPONE QUE EL JUGADOR HA PASADO EL x4, EMPIEZA A ECHAR CHISPAS! https://youtu.be/-mMhKYJFOnQ */
                }

                BonusTimer(); // ...COMIENZA A CORRER EL RELOJ DE TIEMPO DE BONUS...
                StartCoroutine(MultiplierUI()); // ...Y CREA UNA SUBRUTINA INDEPENDIENTE QUE MUESTRE EL MULTIPLICADOR.
            }
            else
            {
                multiplier.color = Color.white; /* SI YA NO ESTÁ EN BONUS TIME, DEVUELVE EL TEXTO DEL MULTIPLICADOR A SU COLOR ORIGINAL. */
                sprinkles.Stop(); /* Y PARA LAS CHISPAS, QUE EL MÓVIL SE MUERE. */
            }
        }
        else
        {
            asMusic.volume = 0.05f; /* SI EL USUARIO ESTÁ EN GAMEOVER O EN PAUSA, BAJA EL VOLUMEN DE LA MÚSICA. PARA QUE NO MOLESTE AL JUGADOR
                                       SI HA TENIDO QUE PARAR LA PARTIDA PARA ATENDER A OTRA COSA MOMENTÁNEAMENTE. O LE ESTÁN HABLANDO. */

            SubControls(); /* ACTIVO LOS SUBCONTROLES, QUE NO SON MÁS QUE LLAMADAS A CONTROLES SUELTOS */
        }
    }

    private void LateUpdate() /* ESTA FUNCIÓN ESPECÍFICA DE UNITY SE EJECUTA DESPUÉS DE UPDATE, SIRVE PARA EL TRACKING DE CÁMARAS Y ALGUNAS COSAS
                                 MÁS, QUE NECESITAN PRECISIÓN DE SEGUIMIENTO, ASÍ QUE PARA ARREGLAR EL GLITCH DEL TETRIMINO PENETRADOR DE PAREDES
                                 CHEQUEO LOS LÍMITES AQUÍ, PARA QUE UNA VEZ QUE EL TETRIMINO ENTRE FUERA DE LA PARED, JUSTO DESPUÉS, LO ESCUPA.
                                 HASTA AHORA NO ME HA DADO MÁS ERRORES...*/
    {
        CheckPlaygroundLimits();
    }

    /* ------------------- FUNCIONES O MÉTODOS DE CLASE PLAYGROUND ------------------- */

    private void GeneratePlayground() // ESTA FUNCIÓN GENERA EL TABLERO SEGUN EL ALTO Y EL ANCHO.
    {
        for (short y = 0; y < alto; y++) // AQUÍ NO HAY MUCHO QUE DECIR... DE 0 HASTA 24...
        {
            for (short x = 0; x < ancho; x++) // DE 0 HASTA 10...
            {
                playground[x, y] = null; // A LA MATRIZ 2D DE VALOR HORIZONTAL "X" Y VERTICAL "Y", INICIALÍZALOS A NULO TODOS.
                /* QUICIR, HAZME UN TABLERO VACÍO DE 10 X 24 HUECOS NULOS, MÁS ADELANTE SE IRÁN LLENANDO DE GameObjects, EN
                 * ESTE CASO LOS MINOS INDIVIDUALES QUE LUEGO PODREMOS ELIMINAR CADA VEZ QUE HAGAMOS LÍNEA.
                 * 
                 LA IDEA ES QUE AL COMENZAR EL JUEGO, SE GENERE UN TETRIMINO. EL TETRIMINO APARECE COMO UN PAQUETE CERRADO, QUE
                 CONTIENE OTROS OBJETOS DENTRO MÁS PEQUEÑOS (CHILDREN O HIJOS), YO LOS LLAMO MINOS. MIENTRAS JUEGAS MUEVES
                 EL PAQUETE ENTERO (PARENT O PADRE) CON LOS CONTROLES, PERO CUANDO UN TETRIMINO SE ESTABLECE, SE DESEMPAQUETA
                 Y DEJA LOS MINOS SUELTOS EN EL TABLERO. ASÍ LUEGO SOLO TENGO QUE RECORRER Playground DE FORMA HORIZONTAL
                 PARA BORRAR UNA LÍNEA DE MINOS SIN QUE MOLESTEN LOS PADRES. MÁS ADELANTE IRÉ MATIZANDO TODO EL CONCEPTO. */
            }
        }
    }

    private void Timer() // ESTE ES EL RELOJ INTERNO DE XENTRIX, GENERA TICS DE UN SEGUNDO.
    {
        elapsedTime += Time.deltaTime; /* VE LLENANDO EL TIEMPO TRANSCURRIDO CON LOS MILISEGUNDOS DE CADA FRAME. 
                                          Time.deltaTime SE USA PARA SINCRONIZAR EL TIEMPO A LA VELOCIDAD DE RELOJ DE LA CPU.
                                          LITERALMENTE DEVUELVE EL TIEMPO QUE HA PASADO DESDE EL FOTOGRAMA ANTERIOR.
                                          ESTO EVITA QUE PARA DIFERENTES DISPOSITIVOS, UN SEGUNDO DURE MÁS O MENOS SEGÚN LA 
                                          VELOCIDAD DE CADA CPU QUE LO EJECUTE! https://youtu.be/Y9DGH_bVz5Y
                                          ASÍ TE ASEGURAS QUE 1 SEGUNDO SEA 1 SEGUNDO EXACTO EN TODOS LOS DISPOSITIVOS. */

        if (elapsedTime >= timerLimit) // SI EL TIEMPO TRANSCURRIDO ES MAYOR O IGUAL A 1s...
        {
            Gravity(); // EJECUTA LA FUNCIÓN GRAVEDAD, QUE TIRA HACIA ABAJO UNA UNIDAD EL TETRIMINO DEL JUGADOR.
            
            // BÁSICAMENTE, TODA FUNCIÓN QUE ESTÉ AQUÍ DENTRO, SE EJECUTARÁ UNA VEZ CADA SEGUNDO. NI MÁS NI MENOS.
        }
    }

    private void BonusTimer() // ESTE ES EL RELOJ QUE CRONOMETRA EL BONUS TIME DE MULTIPLICACIÓN DE PUNTOS.
    {
        elapsedBonusTime += Time.deltaTime; // FUNCIONA PRÁCTICAMENTE IGUAL QUE "Timer()"
        if (elapsedBonusTime >= bonusTimerLimit) // SI EL TIEMPO DE BONUS TRANSCURRIDO ES MAYOR O IGUAL A 2s...
        {
            bonusMultiplier = 0; // REINICIA EL VALOR DE MULTIPLICADOR A 0, PUES SE TE ACABÓ EL TIEMPO.
            elapsedBonusTime = 0f; // REINICIA EL CONTADOR DE SEGUNDOS.
            isBonusTime = false; // ESTABLECE QUE YA NO ESTÁS EN MODO BONIFICACIÓN POR MULTILÍNEA.
        }
    }

    private int ConsecutiveSpawn(bool toggle) /* ESTA FUNCIÓN DES/ACTIVA LA REPETICIÓN CONSECUTIVA DE TETRIMINOS EN
                                                 Spawner(). EN "true" DE FORMA PREDETERMINADA, JUEGO NORMAL.
                                                 AL ESTAR ACTIVA, PERMITE MAYOR PROBABILIDAD DE CONSEGUIR
                                                 MULTIPLICADORES ALTOS, PERO A LA VEZ SE COMPENSA CON QUE PARA
                                                 CONSEGUIRLOS DEBES HACER UNA TORRE ALTA, QUE PUEDE FASTIDIARSE
                                                 SI AL PROGRAMA LE DA POR SPAWNEARTE 5 PIEZAS IGUALES SEGUIDAS QUE NO
                                                 NECESITAS Y HACIÉNDOTE TOCAR EL TECHO.
                                                 DESACTIVARLA HACE EL JUEGO MÁS DIFÍCIL PARA SUBIR NIVELES.
                                                 ¿SE PODRÍA ACTIVAR SIBILINAMENTE EN NIVELES ALTOS?
                                                 https://youtu.be/-oAQDMOm88w */
    {
        int rnd; /* CREO UN ENTERO PARA UN VALOR RANDOM... */

        if (!toggle) /* SI EL VALOR QUE SE PASA POR ARGUMENTOS ES FALSE... QUICIR ESTÁ DESACTIVADO. */
        {
            do /* HAZ... */
            {
                rnd = Random.Range(0, nBlocks); /* SIGUE PILLANDO UN NÚMERO RANDOM... */
            }
            while (blockSelected == rnd); /* MIENTRAS QUE LO QUE ELIJAS SEA IGUAL A LO QUE ELEGISTE ANTERIORMENTE. */

            blockSelected = rnd; /* SI PILLÓ ALGO DIFERENTE A LO ANTERIOR, GUÁRDALO PARA LA PRÓXIMA. */

            return rnd; /* DEVUELVE LO QUE PILLASTE */
        }
        else
        {
            return rnd = Random.Range(0, nBlocks); /* SI ESTÁ ACTIVADO EL MODO CONSECUTIVO "MODO NORMAL"...
                                                      PILLA UN NÚMERO RANDOM COMO SIEMPRE Y DEVUÉLVELO. */
        }
    }


    private Transform RandomTetrimino() // ESTA FUNCIÓN DECIDE ALEATORIAMENTE UN TETRIMINO DE LA MATRIZ DE TETRIMINOS DISPONIBLES.
    {
        return prefabs[ConsecutiveSpawn(isConsecutiveSpawn)]; // ESTA FUNCIÓN DEVUELVE UN Transform. UN GameObject QUE GESTIONAREMOS POR SU POSICIÓN.
    }

    private void Spawner() // ESTA FUNCIÓN GENERA EL TETRIMINO EN EL TABLERO SEGÚN EL TETRIMINO ELEGIDO EN RandomTetrimino().
    {
        newTetrimino = Instantiate<Transform>(RandomTetrimino(), transform); /* INSTANCIAME UN TETRIMINO ALEATORIO Y ME LO METES
                                                                                COMO CHILD O HIJO EN EL OBJETO QUE CONTIENE ESTE
                                                                                SCRIPT. PERO ADEMÁS ME GUARDAS ESA INSTANCIA EN
                                                                                newTetrimino PARA PODER GESTIONARLA MÁS ADELANTE */

        tetriminoHelper = Instantiate<Transform>(newTetrimino, transform); /* CREO UNA COPIA DE newTetrimino PARA HACER EL AYUDANTE. */
        tetriminoHelper.gameObject.SetActive(false); /* DE BUENAS A PRIMERAS, LO DESACTIVO DEL ESCENARIO, NO QUIERO VERLO AÚN. */
        tetriminoHelper.position = newTetrimino.position; /* INICIALIZO EL AYUDANTE EN LA POSICIÓN EXACTA DEL TETRIMINO DEL JUGADOR. */

        minos = new Transform[newTetrimino.childCount]; /* INICIALIZA LA MATRIZ minos CON EL NÚMERO DE VALORES IGUAL AL NÚMERO DE
                                                           MINOS HIJOS QUE TENGA EL TETRIMINO ELEGIDO. ESTO PERMITE QUE PUEDAS CREAR
                                                           CUALQUIER TETRIMINO QUE SE TE OCURRA, CON MÁS DE 4 MINOS INCLUSO. */
        minosHelper = new Transform[minos.Length]; /* COMO AMBOS, TETRIMINO Y AYUDANTE SON LAS MISMAS PIEZAS, INICIALIZA LA MATRIZ DEL AYUDANTE
                                                      PARA SUS MINOS CON EL NÚMERO DE MINOS DE newTetrimino. */


        for (short i = 0; i < newTetrimino.childCount; i++) // BÁSICAMENTE DE 0 A nMINOS...
        {
            minos[i] = newTetrimino.GetChild(i); /* VE METIENDO LOS HIJOS EN UNA MATRIZ DE HIJOS. PARA LUEGO GESTIONARLOS
                                                    INDIVIDUALMENTE. */

            minosHelper[i] = tetriminoHelper.GetChild(i); /* LO MISMO QUE LO ANTERIOR PERO PARA EL AYUDANTE. */
        }

        ConfigTetrimino(); /* FUNCIÓN PARA CONFIGURAR VISUALMENTE EL TETRIMINO DEL JUGADOR. */
        ConfigHelper(); /* LO MISMO, PERO PARA EL AYUDANTE. */
    }

    private void ConfigTetrimino() /* ESTA FUNCIÓN, APLICA EL SPRITE DE LADRILLO A LOS TETRIMINOS. */
    { 
        if (tetrimino_graph != null) /* PRIMERO COMPRUEBO SI ESTÁ DISPONIBLE EL SPRITE, POR SI SE HA TRASCONEJADO, NO DE
                                        ERROR Y NO SE PUEDA JUGAR, SI NO ENCONTRARA EL SPRITE, USARÍA EL COLOR ESTANDAR. */
        {
            for (short i = 0; i < minos.Length; i++) /* POR CADA MINO DEL TETRIMINO... */
            {
                minos[i].GetComponent<SpriteRenderer>().sprite = tetrimino_graph; /* ESTABLECE EL SPRITE A CADA MINO. */
            }
        }
    }

    private void ConfigHelper() /* ESTA FUNCIÓN CAMBIA EL COLOR DE LA COPIA DEL TETRIMINO (AYUDANTE) PARA QUE PAREZCA
                                   UNA SOMBRA.*/
    {
        SpriteRenderer mRend; /* CREO UNA VARIABLE TEMPORAL TIPO SpriteRenderer QUE LA QUE USA UNITY PARA RENDERIZAR LOS
                                 SPRITES. */

        for (short i = 0; i < tetriminoHelper.childCount; i++) /* POR CADA MINO DEL AYUDANTE... */
        {
            mRend = minosHelper[i].GetComponent<SpriteRenderer>(); /* PILLO LA REFERENCIA DEL COMPONENTE SpriteRenderer
                                                                      DE CADA MINO Y LA METO TEMPORALMENTE EN mRend
                                                                      PARA MODIFICARLO. */
            mRend.color = new Color(1, 1, 1, 0.2f); /* LE CAMBIO EL COLOR A BLANCO CON UN ALPHA DEL 20% */
            mRend.sortingOrder = -1; /* sortingOrder SE UTILIZA PARA DETERMINAR EL ORDEN EN CAPAS DE LOS OBJETOS DE LA
                                        ESCENA. ALGO ASÍ COMO LAS CAPAS DE PHOTOSHOP. LO PONGO A -1 PORQUE QUIERO QUE
                                        EL AYUDANTE ESTÉ POR DETRÁS DEL TETRIMINO DEL JUGADOR. */
        }
    }

    private void HelperMimicking() /* ESTA FUNCIÓN IMITA, CUAL MIMO DE MIERDA, EL AYUDANTE AL TETRIMINO DEL JUGADOR. */
    {
        tetriminoHelper.position = newTetrimino.position; /* LA POSICIÓN DEL AYUDANTE ES LA MISMA QUE LA DEL JUGADOR... */
        tetriminoHelper.rotation = newTetrimino.rotation; /* QUE LA ROTACIÓN TAMBIÉN SEA LA MISMA... */

        bool isHit = false; /* ESTA VARIABLE IGUAL NO ES NECESARIA, PERO LOS WHILE EN BUCLE INFINITO ME DAN URTICARIA.
                               ASÍ QUE LES PONGO UN STOP EN FORMA DE BOOL. DIGO QUE NO ES NECESARIA PORQUE LOS "return"
                               CORTAN POR OBLIGACIÓN LOS WHILE Y TODA LA JODIDA FUNCIÓN SI ES NECESARIA. */

        while (!isHit) /* MIENTRAS SEA FALSE... */
        {
            foreach (Transform h in minosHelper) /* POR CADA MINO DEL AYUDANTE... */
            {
                if (h.position.y > 0 && h.position.x < alto && h.position.x > 0) /* SI ESTÁN DENTRO DEL ESCENARIO... */
                {
                    if (playground[Mathf.FloorToInt(h.position.x), Mathf.FloorToInt(h.position.y)] != null) /* SI TOCAN
                                                                                                               UNA PIEZA
                                                                                                               YA
                                                                                                               ESTABLECIDA...*/
                    {
                        tetriminoHelper.position += Vector3Int.up; /* QUE EL AYUDANTE SUBA UNA UNIDAD. */
                        isHit = true; /* DOY LA SALIDA AL WHILE... */
                        return; /* AUNQUE DA IGUAL PORQUE ESTÁ ESTE... https://media1.tenor.com/images/93854108c6920a98072dff147604ec95/tenor.gif */
                    }
                }
                else
                {
                    tetriminoHelper.position += Vector3Int.up; /* SI NO ESTÁ TOCANDO UNA PIEZA, ENTONCES ESTÁ TOCANDO EL BORDE
                                                                  DEL ESCENARIO, PERO LA ACCIÓN SERÁ LA MISMA. PARRIBA.
                                                                  QUE SIEMPRE VAYA HACIA ARRIBA LE DA UN COMPORTAMIENTO
                                                                  ESCURRIDIZO, NO COMO EL TETRIMINO DEL JUGADOR.*/
                    isHit = true; /* SALIDA PLACEBO. */
                    return; /* SALIDA REAL <3 */
                }
            }

            tetriminoHelper.position += Vector3Int.down; /* SI NO SE HA TOPADO CON NADA, QUE BAJE AYUDANTE HASTA QUE LO HAGA.*/
        }
    }

    private void Controls() // ESTA FUNCIÓN DEFINE LAS TECLAS DE CADA ACCIÓN EN PC Y MANDO, SI AHORA PUEDES JUGAR CON MANDO!.
    {
        /* ESTO ES NUEVO, EN LAS ANTERIORES VERSIONES DE XENTRIX RECIBÍAMOS DIRECTAMENTE A LA TECLA DEL TECLADO, PERO UNITY
           TIENE UNA SECCIÓN PROPIA PARA DEFINIR TECLAS EN [EDIT>PROJECT SETTINGS>INPUT], ASIGNÁNDOLES NOMBRES CLAVE Y QUE
           PUEDEN SER REUTILIZADOS POR TODO TIPO DE CONTROLES, ASÍ QUE LO HACE SER MÁS FLEXIBLE. AÚN ASÍ, AHORA HAY UNO NUEVO
           QUE MEJORA LA DEFINICIÓN Y ES AÚN MÁS FLEXIBLE, PERO BUENO. XENTRIX NO ES SIBARITA... */
        if (!isTactile) /* SI ESTAMOS EN UN PC... */
        {
            if (Input.GetButtonDown("Rotate") || Input.GetButtonDown("Joy_Rotate")) // SI PRESIONO ROTAR O EL BOTÓN DEL MANDO X...
                /* NOTA QUE YA NO USAMOS GetKeyDown SI NO GetButtonDown, ESTA PILLA LAS DEFINICIONES CON NOMBRE CLAVE DE LA SECCIÓN
                 * INPUT. EL NOMBRE CLAVE ES UN STRING, "Rotate" ES UNO DE ELLOS Y "Joy_Rotate" TIENE DEFINIDO EL MANDO. DENTRO DE
                 ESTAS DEFINICIONES VIENE ESTABLECIDO LA TECLA O TECLAS QUE SE DEBEN PULSAR DE CUALQUIER DISPOSITIVO PARA QUE
                 FUNCIONE TODO EL TINGLADO */
            {
                Rotate(); // ROTA LA REPRESENTACIÓN GRÁFICA DEL TETRIMINO.
            }
            if (Input.GetButtonDown("Down")) /* POR EJEMPLO ESTE TIENE DEFINIDO "S" O "FLECHA ABAJO" DEL TECLADO PARA IR A ABAJO... */
            {
                Down(); // "S" PARA BAJAR UNA UNIDAD.
            }

            if (Input.GetAxisRaw("Joy_Vertical") < 0 && !isJoyAxisYPressed) /* PERO ESTA, AL SER DEL MANDO Y SER DE LA CRUCETA QUE,
                MI PC TOMA COMO UN JOYSTICK... SI EL "JOYSTICK" TOMA UN VALOR INFERIOR A 0, ES QUE LE ESTOY DANDO A LA IZQUIERDA.
                AQUÍ USAMOS GetAxisRaw, QUE SE USA EXCLUSIVAMENTE PARA LOS JOYSTICKS ANALÓGICOS. */
            {
                isJoyAxisYPressed = true; /* DEBIDO A QUE LOS JOYSTICKS SON ANALÓGICOS Y NO TOMAN PULSACIONES ÚNICAS COMO LOS BOTONES...
                                             DEBO HACER QUE AL PULSAR LA CRUCETA SOLO TOME UNA PULSACIÓN. VAMOS ES COMO AL ESCRIBIR AQUÍ,
                                             QUE DEJAS PULSADO LA A Y: AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA, PUES CON ESTO LO CONVIERTO
                                             EN UNA SOLA PULSACIÓN PARA QUE EL TETRIMINO SE MUEVA DE UNO EN UNO. */
                Down(); /* PABAJO */
            }
            else if (Input.GetAxisRaw("Joy_Vertical") == 0) /* SI NO ESTOY PULSANDO LA CRUCETA EN VERTICAL... ESTÁ EN PUNTO MUERTO, 0*/
            {
                isJoyAxisYPressed = false; /* NO ESTÁ PULSANDO EL EJE Y DE LA CRUCETA */
            }

            if (Input.GetButtonDown("Left"))
            {
                Left(); // "A" O "FLECHA IZQUIERDA" PARA LA IZQUIERDA UNA UNIDAD.
            }

            if (Input.GetAxisRaw("Joy_Horizontal") < 0 && !isJoyAxisXPressed) /* SI LE DOY A LA CRUCETA IZQUIERDA DEL MANDO... Y AÚN NO SE
                HA PRESIONADO... */
            {
                isJoyAxisXPressed = true; /* ESTÁ PRESIONADO */
                Left(); // PA LA IZQUIERDA.
            }
            else if (Input.GetAxisRaw("Joy_Horizontal") > 0 && !isJoyAxisXPressed) /* SI PULSA LA DERECHA DE LA CRUCETA Y NO SE HA PRESIONADO... */
            {
                isJoyAxisXPressed = true; // ESTÁ PRESIONADO
                Right(); // DERECHA
            }
            else if(Input.GetAxisRaw("Joy_Horizontal") == 0) /* SI EL EJE HORIZONTAL DE LA CRUCETA ESTÁ EN PUNTO MUERTO...*/
            {
                isJoyAxisXPressed = false; /* REINÍCIAME LA VARIABLE */
            }

            if (Input.GetButtonDown("Right"))
            {
                Right(); // "D" O "FLECHA DERECHA" PARA LA DERECHA.
            }
            if (Input.GetButtonDown("Drop"))
            {
                Drop(); // "ESPACIO" O "CÍRCULO EN EL MANDO" PARA DEJAR CAER LA PIEZA EN CAÍDA LIBRE.
            }
            if(Input.GetButtonDown("Pause"))
            {
                PauseGame(); // "P" O "START" EN EL MANDO PARA PAUSAR EL JUEGO.
            }
            if (Input.GetButtonDown("Mute"))
            {
                MuteMusic(); // "M" O "SELECT" PARA MUTEAR EL JUEGO.
            }
            if (Input.GetButtonDown("Exit") || (Input.GetButtonDown("Mute") && Input.GetButtonDown("Pause")))
            {
                QuitGame(); // "ESCAPE" O "START" Y "SELECT" A LA VEZ PARA SALIR DEL JUEGO.
            }
        }

        CheckPlaygroundLimits(); /* OPTÉ POR ELIMINAR ESTA FUNCIÓN DEL UPDATE Y PONERLA AQUÍ PARA QUE SOLO CALCULE CUANDO TE MUEVAS,
                                    ASÍ AHORRAMOS MUCHÍSIMA CARGA DE CPU... PARA EL MÓVIL, CLARO, EL PC NI SE INMUTA */
    }

    private void SubControls() /* ESTA NUEVA FUNCIÓN, DEJA ACTIVOS UNOS CUANTOS BOTONES PARA MOVERSE POR LOS MENÚS QUE DE OTRA
        FORMA ESTARÍAN DESACTIVADOS EN PANTALLAS DE GAMEOVER O PAUSE */
    {
        if(!isTactile)
        {
            if (isPaused) /* SI ESTÁ EN PAUSA EL JUEGO */
            {
                if (Input.GetButtonDown("Pause") || Input.GetButtonDown("Drop") || Input.GetButtonDown("Joy_Rotate"))
                    /* Y LE DAMOS A PAUSAR O A ESPACIO O AL CÍRCULO EN EL MANDO... */
                {
                    PauseGame(); // DESPÁUSATE.
                }
            }
            else if (isGameOver) // SI ES GAMEOVER...
            {
                if (Input.GetButtonDown("Pause") || Input.GetButtonDown("Drop") || Input.GetButtonDown("Joy_Rotate"))
                    /* LAS MISMAS TECLAS QUE EL ANTERIOR... */
                {
                    RestartGame(); // REINICIA EL JUEGO.
                }
            }

            if (Input.GetButtonDown("Mute")) // SI LE DAMOS A MUTEAR EN CUALQUIER PANTALLA EMERGENTE...
            {
                MuteMusic(); // MUTEA/DESMUTEA
            }

            if (Input.GetButtonDown("Exit") || (Input.GetButtonDown("Mute") && Input.GetButtonDown("Pause")))
                /* SI LE DAMOS A ESCAPE O START Y SELECT EN LOS MENÚS EMERGENTES... */
            {
                QuitGame(); // QUITA EL JUEGO.
            }
        }
    }

    private void Gravity() // FUNCIÓN GRAVEDAD, CAE LA REPRESENTACIÓN GRÁFICA DEL TETRIMINO UNA UNIDAD PARA ABAJO.
    {
        Down(); /* Gravity() HACE ESENCIALMENTE LO MISMO QUE SI PULSARAS A ABAJO, PERO LO HACE AUTOMÁTICAMENTE CADA SEGUNDO.
                   ASÍ QUE ME AHORRO REDUNDANCIAS DE CÓDIGO SI PONGO LA FUNCIÓN DEL BOTÓN ABAJO */
    }

    private void CheckGameOver(int y) // FUNCIÓN QUE COMPRUEBA SI ESTAMOS EN UNA SITUACIÓN DE GAME OVER.
    {
        if(y > alto -5) /* LE PASAMOS POR ARGUMENTOS LA LÍNEA QUE OCUPA CADA MINO EN VERTICAL AL ESTABLECER LA PIEZA Y, SI SOBREPASA
                      EL TECHO... */
        {
            isGameOver = true; // ESTAMOS EN GAME OVER, HAS MORIDO.
            gameOver.SetActive(isGameOver); // ACTIVA LA PANTALLA DE GAME OVER PARA QUE EL JUGADOR LO VEA Y SE LE CAIGA EL MUNDO.
            ToggleButtons(!isGameOver); // Y DESACTIVA LOS BOTONES DEL JUEGO QUE YA NO TE HARÁN FALTA.
        }
    }

    private void UpdatePlayground() // ESTA FUNCIÓN ESTABLECE DEFINITIVAMENTE LOS TETRIMINOS SOBRE EL playground.
    {
        for (short i = 0; i < minos.Length; i++) // POR CADA MINO DEL TETRIMINO...
        {
            CheckGameOver(Mathf.FloorToInt(minos[i].position.y)); /* MIRA A VER SI ALGÚN MINO ESTÁ EN ZONA DE GAME OVER, HAZ EL FAVOR.
                                                                     Mathf.FloorToInt() BÁSICAMENTE TRUNCA LOS VALORES AL ENTERO MÁS
                                                                     CERCANO, POR SI ALGUNA PIEZA ES DE ESAS CON POSICIÓN NO ENTERA */

            newTetrimino.GetChild(0).SetParent(transform); /* CONVIERTE EL HIJO MINO DEL TETRIMINO PADRE, EN HIJOS DEL GameObject
                                                              EN EL QUE ESTÁ ESTE SCRIPT, QUICIR, SÁCA LOS HIJOS DE DENTRO DEL
                                                              PADRE. */   

            playground[Mathf.FloorToInt(minos[i].position.x), Mathf.FloorToInt(minos[i].position.y)] = minos[i].gameObject;
            /* *INHALAR PROFUNDO* ESTABLECE DEFINITIVAMENTE LA POSICIÓN DE CADA MINO DEL TETRIMINO DEL JUGADOR EN EL ESCENARIO,
             * A LA POSICIÓN HOMÓLOGA ENTERA DE LA MATRIZ playground. *EXHALACIÓN PROFUNDA* */
        }

        Destroy(tetriminoHelper.gameObject); /* DESTRUYO EL AYUDANTE POR COMPLETO PUESTO QUE A ESTE NO LO TENEMOS QUE MANTENER EN
                                                EL ESCENARIO. */

        Destroy(newTetrimino.gameObject); /* EL PAQUETE DEL TETRIMINO PADRE HA QUEDADO VACÍO, ES INVISIBLE
                                             (PORQUE NO CONTIENE NADA), PERO AÚN ASÍ, PARA QUE NO CONSUMA RECURSOS,
                                             LO ELIMINAMOS DE LA EXISTENCIA. */
            
        StartCoroutine(CheckForLineStrikes()); /* INICIAMOS UNA SUBRUTINA INDEPENDIENTE QUE BUSCARÁ SI UNA VEZ ESTABLECIDO EL
                                                  TETRIMINO EN playground, HEMOS HECHO UNA LÍNEA */

        ScoreUp(BLOCKVALUE); // COMO HEMOS ESTABLECIDO UN TETRIMINO, GANAMOS 100 PUNTICOS.
        Spawner(); // AL ESTABLECER UN TETRIMINO, GENERAMOS OTRO AUTOMÁTICAMENTE.
    }

    private void ScoreUp(int value) // ESTA FUNCIÓN ADMINISTRA LOS PUNTOS GANADOS.
    {
        scoreValue += value; // A LA PUNTUACIÓN TOTAL LE VAMOS SUMANDO EL VALOR DE PREMIO QUE LE PASAMOS POR ARGUMENTOS.
        score.text = scoreValue.ToString(); // AL CAMPO DE TEXTO QUE REPRESENTA GRÁFICAMENTE LA PUNTUACIÓN, ACTUALIZAMOS SU VALOR.
        LevelingFromScore(); // COMPROBAMOS SI SEGÚN NUESTRA PUNTUACIÓN TOTAL, MERECEMOS SUBIR DE NIVEL O QUÉ.
    }

    private IEnumerator CheckForLineStrikes() /* IEnumerator. ESTO ES UNA CORRUTINA. UNA CORRUTINA SE EJECUTA AL MARGEN
                                                 DEL FLUJO NORMAL DE LA APLICACIÓN, PUEDE SER INSTANCIADA DE FORMA PARALELA Y EL
                                                 FLUJO DE SU EJECUCIÓN PUEDE CONTROLARSE CON "yield". EN ESTA OCASIÓN, ESTA
                                                 FUNCIÓN QUE DEVUELVE UN IEnumerator, COMPRUEBA SI HEMOS HECHO LÍNEAS HORIZONTALES.
                                                 LAS CORRUTINAS DEBEN TENER SI O SI "yield" EN ALGÚN LADO O DARÁ ERROR */
    {
        for (short y = 0; y < alto - 3; y++) // VERTICALMENTE, DE ABAJO A ARRIBA, DE 0 AL MARCO SUPERIOR...
        {
            bool isNull = false; // CREO UNA COMPROBACIÓN DE SI ES NULO. PARA SABER SI LA LÍNEA HORIZONTAL ESTÁ LLENA O NO DE MINOS.

            for (short x = 0; x < ancho; x++) // HORIZONTALMENTE, DE 0 AL ANCHO...
            {
                if (playground[x, y] == null) // SI ALGUNA POSICIÓN DE UNA DETERMINADA LÍNEA HORIZONTAL TIENE UN HUECO VACÍO...
                {
                    isNull = true; // HAY UN HUECO VACÍO.
                    break; // SÁLTATE ESTE FOR Y NO SIGAS CONTANDO. SOLO ES NECESARIO UN NULO PARA DESCARTAR LA LÍNEA ENTERA.
                }
            }

            if (!isNull) // SI isNull SIGUE EN false SIGNIFICA QUE LA LÍNEA ESTÁ LLENA. ASÍ QUE...
            {
                isBonusTime = true; // IT'S BONUS TIME!

                for (short x = 0; x < ancho; x++) // CADA MINO DE ESTA LÍNEA ACTUAL LLENA...
                {
                    SpriteRenderer sr; /* CREO UNA VARIABLE TEMPORAL... */
                    sr = playground[x, y].GetComponent<SpriteRenderer>(); /* METO LA REFERENCIA DEL SPRITE DE CADA MINO... */
                    sr.color = Color.white; // ME LO CAMBIAS DE COLOR A BLANCO
                }

                yield return new WaitForSeconds(lineSpeed); /* LITERALMENTE, PÁRATE DURANTE UNOS MILISEGUNDOS PARA QUE EL JUGADOR
                                                               PUEDA VER QUE HAS CAMBIADO DE COLOR. EN CIRCUNSTANCIAS NORMALES
                                                               LA CPU VA TAN RÁPIDO QUE NO VERÍAS NADA... 
                                                               EL yield return new WaitForSeconds(X) FUNCIONA COMO UN
                                                               STOP O WAIT DE OTROS LENGUAJES, DETIENE LA EJECUCIÓN DEL CÓDIGO X
                                                               SEGUNDOS. */

                SoundFX[1].Play(); /* LE DOY AL PLAY AL SONIDO DE HACER LÍNEA DE LA MATRIZ DE EFECTOS. */

                for (short x = 0; x < ancho; x++) // LUEGO DE CAMBIAR EL COLOR DE CADA MINO, DA OTRA PASADA A LA LÍNEA.
                {
                    Destroy(playground[x, y].gameObject); // Y CÁRGATE LAS REPRESENTACIONES GRÁFICAS, OJO, DE LOS MINOS.
                    playground[x, y] = null; // MARCAMOS LA LÍNEA COMO NULOS.
                }

                yield return new WaitForSeconds(lineSpeed); // PÁRATE OTRA VEZ PARA QUE VEAN COMO DESAPARECE LA LÍNEA.

                bonusMultiplier++; // COMO HEMOS HECHO UNA LÍNEA, AUMENTAMOS x1 EL MULTIPLICADOR.
                elapsedBonusTime = 0f; // POR CADA LÍNEA QUE HAGAMOS, VOLVEMOS A DARTE 2s COMPLETOS DE TIEMPO DE BONUS.

                SoundFX[2].Play(); /* PLAY AL SONIDO DEL MULTIPLICADOR SUBIENDO */
                SoundFX[2].pitch = (float)(bonusMultiplier / 3f) + 1f; /* ESTE CHORIZO LO QUE HACE SIMPLEMENTE ES IR HACIENDO
                                                                          MÁS AGUDO EL SONIDO DEL MULTIPLICADOR, CUANTO MÁS
                                                                          VALOR DE MULTIPLICADOR CONSIGAS. LE DA UN TOQUE DE
                                                                          EPICIDAD Y SI LO MEZCLAS CON LOS CAMBIOS DE COLOR
                                                                          Y EL SISTEMA DE PARTÍCULAS CON CHISPAS...
                                                                          EL ALGORITMO PARECE MUY LIOSO MATEMÁTICAMENTE PERO
                                                                          SOLO HACE QUE LA SUBIDA DE FRECUENCIA SEA MÁS SUAVE.
                                                                          CON EL (float) ME ASEGURO QUE DEVUELVE UN FLOAT, 
                                                                          HACIENDO UNA CONVERSIÓN IMPLÍCITA A FLOAT. POR SI LAS
                                                                          FLYS. https://youtu.be/uslW3CFVEHg */

                BlockFalling(y); /* ESTA FUNCIÓN SE ENCARGA DE COGER TODOS LOS MINOS QUE ESTÁN ENCIMA DE LA LÍNEA ELIMINADA Y BAJARLA
                                    COMO SI LA GRAVEDAD LES AFECTASE */
                totalLines++; // LÍNEAS TOTALES +1, LAS CUENTO PERO NO LAS USO NI LAS REPRESENTO... PARA UN DLC YA SI ESO.
                ScoreUp(LINEVALUE * bonusMultiplier); /* AÑADIMOS A LA PUNTUACIÓN EL VALOR DE UNA LÍNEA MULTIPLICADO POR EL
                                                         MULTIPLICADOR OBTENIDO. LA PRIMERA LÍNEA NO CUENTA, COMO ES LÓGICO. */

                yield return StartCoroutine(CheckForLineStrikes()); // https://youtu.be/zeft9c6CITs

                /* ESTO ES UNA FUNCIÓN RECURSIVA, UNA FUNCIÓN RECURSIVA ES UNA QUE SE LLAMA A SÍ MISMA, COMO UNA MATRIOSHKA
                   SE PUEDE HACER SIN PROBLEMAS, PERO DEBES TENER EN CUENTA QUE DEBES HACERLE UNA SENTENCIA QUE CORTE LA EJECUCIÓN
                   POR UN LADO O SI NO, SE CONVERTIRÁ EN UN BUCLE INFINITO TO WAPO.
                   PERO ESTO ES ALGO DIFERENTE, ES UNA CORRUTINA RECURSIVA QUE ES LLAMADA EN UN yield. EN TEORÍA SI LO METES DENTRO
                   DE UN yield, ESTA CORRUTINA SE EJECUTA UNA VEZ QUE LA ANTERIOR EJECUCIÓN TERMINE. ES DECIR QUE SE ESPERA A QUE
                   ACABE. ESO ESTÁ BIEN PARA EVITAR QUE AMBAS INSTANCIAS DE LA CORRUTINA SE PISEN LA UNA A LA OTRA. */
            }
        }

        StopCoroutine(CheckForLineStrikes()); /* UNA VEZ COMPROBADO TOÍTO, SUICÍDATE UN RATICO. LA CORRUTINA SE PARA A SÍ MISMA.
                                                 LAS CORRUTINAS HAY QUE PARARLAS EXPLÍCITAMENTE, PORQUE QUEDAN EN EJECUCIÓN
                                                 INDEFINIDAMENTE, AUNQUE NO HAGAN NADA. Y ESTO ADEMÁS, ES LA SENTENCIA DE CORTE
                                                 DE LA RECURSIVIDAD */
    }

    private IEnumerator MultiplierUI() // OTRA CORRUTINA, ESTA VEZ PARA CONTROLAR EL TESTIGO DEL MULTIPLICADOR. ESTÉTICO.
    {
        if (bonusMultiplier > 1) // SI EL VALOR DE MULTIPLICADOR ES MAYOR QUE 1...
        {
            multiplier.gameObject.SetActive(true); // ACTIVO EL TESTIGO GRÁFICO DE MULTIPLICADOR PARA QUE EL JUGADOR LO VEA.

            multiplier.text = ("x" + bonusMultiplier.ToString()); // EL TEXTO DEL MULTIPLICADOR QUE SEA x2, x3, x4 ETC...
            multiplier.fontSize = bonusFontSize; // INICIALIZO A LOS VALORES POR DEFECTO DEL TAMAÑO DEL TEXTO.
            multiplier.fontSize += 20 * bonusMultiplier; /* AHORA LE SUMO 20 PUNTOS MULTIPLICADO POR EL VALOR DE MULTIPLICADOR.
                                                            CUANTO MÁS ALTO SEA EL MULTIPLICADOR MÁS GRANDE SERÁ LA FUENTE.*/

            while (isBonusTime) // LAS CORRUTINAS SUELEN USAR BUCLES CONTROLADOS POR yield PARA FUNCIONAR.
            {
                multiplier.transform.position = vectorOrigin; // INICIALIZO LA POSICIÓN DEL TESTIGO A SU POSICIÓN INICIAL.
                multiplier.transform.position += new Vector3(Random.Range(-0.05f, 0.05f) * bonusMultiplier, Random.Range(-0.05f, 0.05f) * bonusMultiplier, 0);
                /* BÁSICAMENTE HAGO VIBRAR EL TESTIGO DE FORMA ALEATORIA DENTRO DE UNOS PEQUEÑOS RANGOS ESTABLECIDOS Y LOS
                 * AUMENTO CUANTO MAYOR SEA EL VALOR DE MULTIPLICADOR. */

                yield return new WaitForSeconds(lineSpeed); // LO PARO UN POCO PARA NO EJECUTARLO CON CADA CICLO DE CPU.
            }

            multiplier.transform.position = vectorOrigin; /* VUELVO A LLEVAR EL TESTIGO A SU PUNTO DE ORIGEN. */
            multiplier.gameObject.SetActive(false); // Y SE DESACTIVA PARA QUE NO LO VEA EL JUGADOR.
        }

        StopCoroutine(MultiplierUI()); // COMO EN LA ANTERIOR CORRUTINA, CUANDO TERMINE DE SU TRABAJO, SUICIDIO.
    }

    private void BlockFalling(short y) // LA FUNCIÓN QUE SE ENCARGA DE BAJAR LOS MINOS SUPERIORES A UNA LÍNEA ELIMINADA.
    {
        for (short i = y; i < alto - 3; i++) /* LE PASO LA POSICIÓN VERTICAL DE LA LÍNEA ELIMINADA POR ARGUMENTOS Y A PARTIR DE AHÍ
                                              HACIA ARRIBA HASTA EL BORDE SUPERIOR. */ 
        {
            for (short j = 0; j < ancho; j++) // HORIZONTALMENTE...
            {
                playground[j, i] = playground[j, i + 1]; /* BÁSICAMENTE, HAZ QUE LA POSICIÓN QUE HA QUEDADO VACANTE, SE LLENE
                                                            CON UN CLON DE LA POSICIÓN QUE TIENE ENCIMA.*/

                if(playground[j, i + 1] != null) // SI LO QUE HAY ENCIMA NO ES NULO... ES DECIR, HEMOS CLONADO EL MINO SUPERIOR...
                {
                    playground[j, i + 1].transform.position += Vector3Int.down; /* AHORA TENEMOS QUE BAJAR GRÁFICAMENTE LOS MINOS
                                                                                   SUPERIORES. SI NO COMPROBAMOS SI NO ES NULO,
                                                                                   NOS DARÁ ERROR PORQUE ESTAMOS INTENTANDO BAJAR
                                                                                   FÍSICAMENTE ALGO INEXISTENTE. */
                }
            }
        }
    }

    private void LevelingFromScore() // ESTA FUNCIÓN SE ENCARGA DE REFLEJAR EL NIVEL DEPENDIENDO DE LA PUNTUACIÓN GANADA.
    {
        int levelTimes = Mathf.FloorToInt(scoreValue / LEVELVALUE); /* DIVIDIMOS EN UNA VARIABLE TEMPORAL LA PUNTUACIÓN TOTAL POR
                                                                       LA PUNTUACIÓN NECESARIA PARA PASAR NIVEL. ESO NOS DA LAS
                                                                       VECES QUE HEMOS SUPERADO LOS 10.000 PUNTOS DE UNA SENTADA */  

        if(nTimesLevelScore < levelTimes) // SI LAS VECES QUE HEMOS PASADO LOS 10000 PTS ES MENOR QUE LAS VECES ACTUALES...
        {
            for (short i = 0; i < levelTimes-nTimesLevelScore; i++) /* EN TEORÍA ESTO DETERMINA SI HEMOS PASADO MÁS DE UN NIVEL
                                                                       A LA VEZ... SI, HAY QUE JUGAR COMO UN JAPONÉS FLIPAO PARA
                                                                       QUE GANES LOS SUFICIENTES PUNTOS DEL TIRÓN COMO PARA SUBIR
                                                                       MÁS DE UN NIVEL A LA VEZ... PERO BUENO AHÍ ESTÁ, POR SI ACASO.
                                                                       PARA CONSEGUIR ESTO, EL JUGADOR DEBE SER CAPAZ DE OBTENER
                                                                       UN MULTIPLICADOR DE x6 = 21300 pts. DIFÍCIL PERO POSIBLE. */
            {
                LevelUp(); // AUMENTA DE NIVEL TANTAS VECES COMO NIVELES DE UNA SENTADA SE HAYAN PASADO... O NO, SOLO UN NIVEL.
            }

            nTimesLevelScore = levelTimes; /* IGUALO LAS VARIABLES PARA REINICIAR LA COMPARACIÓN. SI NO HAGO ESTO, PODRÍA ESTAR
                                              CONTANDO CADA VEZ MÁS LÍNEAS A LA VEZ. AUNQUE SOLO HAGAS UNA */
        }
    }

    private void LevelUp() // ESTA FUNCIÓN HACE SUBIR DE NIVEL.
    {
        if(timerLimit > 0) // SI EL TIEMPO QUE TARDA EN HACER UN TIC EL RELOJ DE XENTRIX, ES MAYOR A 0...
        {
            timerLimit -= speedUpTime; /* REDUZCO EL TIEMPO DE LA EJECUCIÓN DE UN TIC, AHORA EL SEGUNDO
                                          ES MÁS RÁPIDO Y POR LO TANTO, EL JUEGO.*/
        }

        if(currentLevel < maxLevel) // SI EL NIVEL ACTUAL ES MENOR QUE EL MÁXIMO NIVEL (99)...
        {
            currentLevel++; // AUMENTO EL NIVEL A 1.

            if (currentLevel % 10 == 0) /* "%" ESTA OPERACIÓN SE DENOMINA MÓDULO. DIVIDE EL DIVIDENDO POR EL DIVISOR PERO EN
                                           LUGAR DE DEVOLVER EL COCIENTE, NOS DEVUELVE EL RESTO. LO USO PARA HALLAR LOS DIVISORES
                                           DE UN NÚMERO, QUE SON AQUÉLLOS QUE DIVIDIDOS, DAN EXACTO. EN ESTE CASO LO USO PARA
                                           MARCAR LAS VECES QUE EL NIVEL ACTUAL LLEGA A LA DECENA. QUICIR 10, 20, 30, 40, ETC...
                                           SI EL NIVEL ACTUAL DIVIDIDO POR 10 SU RESTO DA EXACTO (0), ESTÁS EN UNA DECENA */
            {
                unidades = 0; // ACABA DE CAMBIAR DE DECENAS, ASÍ QUE LAS UNIDADES LAS RESETEO A 0.
                lvlUnidades.text = unidades.ToString(); // ACTUALIZO EL TEXTO GRÁFICO EN EL JUEGO EN LAS UNIDADES.

                decenas++; // DECENAS +1
                lvlDecenas.text = decenas.ToString(); // Y ACTUALIZO EL TEXTO GRÁFICO EN EL JUEGO DE LAS DECENAS.
            }
            else // EN CASO CONTRARIO...
            {
                unidades++; // AUMENTO LAS UNIDADES A 1
                lvlUnidades.text = unidades.ToString(); // Y ACTUALIZO EL TEXTO DEL JUEGO DE LAS UNIDADES.
            }
        }
    }

    private void CheckPosition(string dir) /* ESTA FUNCIÓN HA SIDO REMODELADA EN ESTE UPDATE.
        AHORA ES MÁS EFICIENTE Y SIMPLE. ELEGANTE. FUNCIONA COMO UN RELOJ.
        BÁSICAMENTE COMPRUEBA SI EL TETRIMINO DEL JUGADOR SE TOPA CON TETRIMINOS ESTABLECIDOS EN EL ESCENARIO
        SEGÚN LA DIRECCIÓN QUE HA TOMADO EL JUGADOR. */
    {
        if(dir.Equals("Left")) /* SI EL JUGADOR LE DA A LA IZQUIERDA... */
        {
            foreach(Transform m in minos) /* POR CADA MINO DEL TETRIMINO DEL JUGADOR HAZ... */
            {
                if (m.position.x > 0) /* COMPRUEBA QUE NINGÚN MINO SE SALE DEL BORDE IZQUIERDO... */
                {
                    if (playground[Mathf.FloorToInt(m.position.x), Mathf.FloorToInt(m.position.y)] != null)
                        /* COMPRUEBA SI TOCA ALGÚN TETRIMINO YA ESTABLECIDO... */
                    {
                        newTetrimino.position += Vector3Int.right; /* Y COMO EL JUGADOR LE DIO A LA IZQ. CONTRARRESTA
                                                                      HACIA LA DERECHA. AMBAS FUERZAS SE ANULAN Y EL
                                                                      TETRIMINO QUEDA QUIETO. HACIENDO QUE PAREZCA QUE
                                                                      SE HA TOPADO CONTRA UN MURO.*/
                        break; /* break SIRVE PARA ROMPER LOS BUCLES Y SALIRSE DE ELLOS SIN MÁS ITERACIONES INNECESARIAS. */
                    }
                }
            }
        }
        else if(dir.Equals("Right")) /* SI EL JUGADOR LE DA A LA DERECHA... */
        {
            foreach (Transform m in minos) /* POR CADA MINO... */
            {
                if (m.position.x < ancho) /* COMPRUEBA SI NO HA TOPADO CON EL BORDE DERECHO DEL ESCENARIO... */
                {
                    if (playground[Mathf.FloorToInt(m.position.x), Mathf.FloorToInt(m.position.y)] != null)
                        /* SI TOCA UN TETRIMINO ESTABLECIDO... */
                    {
                        newTetrimino.position += Vector3Int.left; /* A LA IZQUIERDA PARA COMPENSAR... */
                        break;
                    }
                }
            }
        }

        if(dir.Equals("Down")) /* SI SE DA HACIA ABAJO...
            SI TE DAS CUENTA, DERECHA E IZQUIERDA VAN JUNTOS CON UN "else if" ESO ES PORQUE NO SE PUEDE IR A LA IZQUIERDA
            Y DERECHA A LA VEZ, PERO EL JUGADOR PUEDE IR A LA DERECHA Y ABAJO AL MISMO TIEMPO, POR ESO LAS SIGUIENTES
            CONDICIONALES ESTÁN SEPARADAS DE FORMA INDIVIDUAL. */
        {
            foreach (Transform m in minos)
            {
                if(m.position.y > 0) /* LOS MINOS NO ESTÁN TOCANDO EL SUELO? */
                {
                    if (playground[Mathf.FloorToInt(m.position.x), Mathf.FloorToInt(m.position.y)] != null)
                        /* SI TOCA UN TETRIMINO CUANDO VA ABAJO... */
                    {
                        newTetrimino.position += Vector3Int.up; /* PARRIBA PARA QUE NO INTERSECTEN. */
                        UpdatePlayground(); /* ESTABLECE EL TETRIMINO EN EL ESCENARIO... */
                        break;
                    }
                }
            }
        }

        if(dir.Equals("Rotate")) /* SI EL JUGADOR ROTA... */
        {
            foreach (Transform m in minos)
            {
                if (m.position.y > 0 && m.position.x > 0 && m.position.x < ancho) /* COMO LA ROTACIÓN ABARCA
                    MÁS ESPACIO PARA GOLPEARSE CON CUALQUIER MIERDA, COMPROBAMOS QUE NO SE SALE DE NINGÚN LÍMITE.*/
                {
                    if (playground[Mathf.FloorToInt(m.position.x), Mathf.FloorToInt(m.position.y)] != null)
                        /* Y QUE GOLPÉE CON OTRO TETRIMINO. */
                    {
                        newTetrimino.Rotate(Vector3.forward, -90f); /* SI LO HACE, QUE EL GIRO SE CONTRARRESTE.
                        EN ESPACIOS ESTRECHOS BÁSICAMENTE NO GIRA. */
                        break;
                    }
                }
                else
                {
                    CheckPlaygroundLimits(); /* SI ESTÁ FUERA DEL BORDE (COSA QUE DE HECHO AL GIRAR ES HABITUAL)
                                                QUE LA FUNCIÓN DE CHEQUEO DE BORDES HAGA SU MAGIA Y LO ESCUPA
                                                DENTRO DE LOS BORDES OTRA VEZ.*/
                }
            }

            if(tetrimino_graph != null) /* ESTO ES PARA EL SPRITE DE LADRILLO DE LOS TETRIMINOS.
                HABÍA UN PROBLEMA. LOS LADRILLOS TIENEN LUCES Y SOMBRAS PINTADAS, PERO AL GIRAR
                EL TETRIMINO, EL DIBUJO GIRA CON ÉL Y LAS "LUCES" SE VAN A LA PUTA POR ALLÁ A LA ARBOLEDA.
                LA SOLUCIÓN ES QUE POR CADA GIRO, DECIRLE AL SPRITE QUE SE REORIENTE CON EL MUNDO... CHACHO*/
            {
                foreach (Transform m in minos) /* POR CADA MINO... */
                {
                    m.rotation = Quaternion.identity; /* GÍRAME EL MINO A LA MATRIZ DE GIRO "IDENTIDAD"
                    BÁSICAMENTE EL GIRO ORIGEN ESTANDAR DEL OBJETO, PERFECTAMENTE ALINEADO CON EL MUNDO.
                    EA, LADRILLOS PERFECTOS EN CADA GIRO!. */
                }
            }

            foreach (Transform h in minosHelper) /* VAMOS HACER LO MISMO QUE CON EL TETRIMINO DEL JUGADOR PERO
                CON EL AYUDANTE, PUES ESTA SOMBRA TAMBIÉN SE SALE DEL ESCENARIO E INTERSECTA CON OTROS TETRIMINOS.*/
            {
                if (h.position.y > 0 && h.position.x > 0 && h.position.x < ancho) /* SI ESTÁ DENTRO DEL ESCENARIO... */
                {
                    if (playground[Mathf.FloorToInt(h.position.x), Mathf.FloorToInt(h.position.y)] != null)
                        /* Y TOCA A OTRO... */
                    {
                        newTetrimino.position += Vector3Int.up; /* PARRIBA DE FORMA ESCURRIDIZA. */
                        break;
                    }
                    else
                    {
                        CheckPlaygroundLimits(); /* SI SE SALE DEL ESCENARIO, QUE SE VUELVA A METER. */
                    }
                }
            }
        }

        if(dir.Equals("Drop")) /* DEJAR CAER EL TETRIMINO. */
        {
            bool isHit = false; /* OTRA VARIABLE PLACEBO. */

            SoundFX[0].Play(); /* AL DEJAR CAER LA PIEZA, QUE SUENE EL GOLPE */

            while (!isHit) /* SI ES FALSO... */
            {
                foreach (Transform m in minos) /* POR CADA MINO DE TETRIMINO... */
                {
                    if (m.position.y > 0) /* SI NO HA TOPADO CON EL SUELO... */
                    {
                        if (playground[Mathf.FloorToInt(m.position.x), Mathf.FloorToInt(m.position.y)] != null)
                            /* Y HA TOCADO UN COMPAÑERO... 016. NO! */
                        {
                            newTetrimino.position += Vector3Int.up; /* PARRIBA */
                            UpdatePlayground(); /* PONLO EN playground COMO NUEVO TETRIMINO ESTABLECIDO... */

                            isHit = true; /* SALIDA DE MIERDA. */
                            return; /* SALIDA TOTAL DE LA FUNCIÓN, PORQUE NO TE DEJA TOCAR MÁS. */
                        }
                    }
                    else
                    {
                        isHit = true;
                        return; /* AQUÍ NO HAY NADA, QUE SE SALGA, PORQUE SI TOCA EL SUELO YA ENTRA
                        EN LA JURISDICCIÓN DE CheckPlaygroundLimits() */
                    }
                }

                newTetrimino.position += Vector3Int.down; /* SI NO TOCA CON NADA, BAJA EL TETRIMINO HASTA QUE LO HAGA. */
            }
        }
    }

    private void CheckPlaygroundLimits() /* ESTA FUNCIÓN COMPRUEBA SI EL TETRIMINO DEL JUGADOR SE SALE DE LOS LÍMITES DEL
                                            ESCENARIO Y LO CORRIGE SI ES ASÍ. ESTA TAMBIÉN HA SIDO REMODELADA.*/
    {
        foreach (Transform m in minos) /* POR CADA MINO DE TETRIMINO */
        {
            if (m.position.x < 0) /* SI TOCA EL BORDE IZQUIERDO... */
            {
                newTetrimino.position += Vector3Int.right; /* PARA LA DERECHA, AL REDIL DEL ESCENARIO. */
                break;
            }
            else if (m.position.x > ancho) /* SI TOCA EL BORDE DERECHO... */
            {
                newTetrimino.position += Vector3Int.left; /* A LA IZQUIERDA, AL REDIL. */
                break;
            }
            
            if (m.position.y < 0) /* COMO EL JUGADOR PUEDE TOCAR LA ESQUINA INFERIOR DERECHA O IZQUIERDA...
                PUEDE SALIRSE POR ABAJO Y POR LOS LADOS, ASÍ QUE COMPRUEBO AMBOS DE FORMA INDEPENDIENTE...*/
            {
                newTetrimino.position += Vector3Int.up; /* SI TOCA EL SUELO, PARRIBA AL REDIL. */
                UpdatePlayground(); /* Y AÑÁDAMELO CON LOS OTROS TETRIMINOS. */
                break;
            }
        }

        foreach(Transform h in minosHelper) /* EL AYUDANTE TAMBIÉN SE PUEDE IR PARA ABAJO... ASÍ QUE LO PROPIO. */
        {
            if (h.position.y < 0)
            {
                tetriminoHelper.position += Vector3Int.up;
                break;
            }
        }
    }

    private void KillMinos() /* ESTA FUNCIÓN SE CARGA TODO GameObject HIJO DEL GameObject PRINCIPAL LLAMADO Grid, QUE USO COMO 
                                CONTENEDOR DE TETRIMINOS Y MINOS GRÁFICOS, COMO MÉTODO DE ORDENACIÓN. LA UTILIZO PARA REINICIAR.
                                EL TABLERO. */
    {
        for (short y = 0; y < this.transform.childCount; y++) /* DE 0 A CANTIDAD DE HIJOS DE Grid QUE HAGO REFERENCIA CON this
                                                                 PORQUE ESTE SCRIPT ESTÁ IMPLEMENTADO EN Grid...*/
        {
            Destroy(this.transform.GetChild(y).gameObject); // DESTRUYE TODOS LOS GameObjects QUE HAYA. NO DEJES "TÍTERE CON TINIEBLA" - PaquiQuotes(c) 2020
        }
    }

    private void HidePhoneControls() /* ESTA NUEVA FUNCIÓN, ELIMINA LOS BOTONES DE PANTALLA TÁCTIL DEL ESCENARIO SI JUGAMOS EN PC */
    {
        left.gameObject.SetActive(false); // AUTOEXPLICATIVOS
        right.gameObject.SetActive(false);
        down.gameObject.SetActive(false);
        drop.gameObject.SetActive(false);
        rotate.gameObject.SetActive(false);
    }

    private void ToggleButtons(bool t) // ESTA FUNCIÓN ACTIVA/DESACTIVA LA FUNCIONALIDAD DE LOS BOTONES PARA MÓVILES.
    {
        left.interactable = t; // EL ESTADO DE LOS BOTONES ENTRA POR ARGUMENTOS Y ACTÚA COMO UN INTERRUPTOR.
        right.interactable = t;
        down.interactable = t;
        drop.interactable = t; // interactable AL DESACTIVARLO NO AFECTA LA VISIBILIDAD DEL GRÁFICO, SIGUEN ESTANDO, PERO NO VA.
        rotate.interactable = t;
        pause.interactable = t;
        quitGame.SetActive(!t); // ESTE DE AQUÍ TOMA LO CONTRARIO.
    }

    public void Rotate() // FUNCIÓN DE ROTACIÓN DEL TETRIMINO DEL JUGADOR.
    {
        newTetrimino.Rotate(Vector3.forward, 90f); /* GÍRAME 90 UNIDADES EL TETRIMINO EN EL EJE Z */
        CheckPosition("Rotate"); /* COMPRUEBA LAS COLISIONES PARA ROTACIÓN */
    }

    public void Down() // FUNCIÓN DE BAJADA DE TETRIMINO.
    {
        newTetrimino.transform.position += Vector3.down; // MUEVE UNA UNIDAD ABAJO.
        CheckPosition("Down"); /* COMPRUEBA LAS COLISIONES PARA ABAJO */
        elapsedTime = 0f; // PON A 0 EL RELOJ PARA QUE NO SE SOLAPEN LAS BAJADAS DEL TIEMPO Y DEL USUARIO Y CUENTEN POR DOS...
    }

    public void Left() // FUNCIÓN DE MOVER IZQUIERDA EL TETRIMINO.
    {
        newTetrimino.transform.position += Vector3.left; // MUEVE UNA UNIDAD A LA IZQUIERDA.
        CheckPosition("Left"); /* COLISIONES PARA IZQUIERDA. */
    }

    public void Right() // LO MISMO PERO A LA DERECHA.
    {
        newTetrimino.transform.position += Vector3.right;
        CheckPosition("Right"); /* COLISIONES PARA DERECHA */
    }

    public void Drop() // FUNCIÓN DE CAIDA LIBRE DE TETRIMINO.
    {
        CheckPosition("Drop"); /* COLISIONES PARA DEJAR CAER. */
        elapsedTime = 0f; // RESETEO DE RELOJ PARA QUE NO SE SOLAPE LA BAJADA CON LA CAIDA.
    }

    public void PauseGame() // FUNCIÓN QUE PAUSA EL JUEGO.
    {
        isPaused = !isPaused; // INTERCAMBIA LOS VALORES DE ESTADO COMO UN INTERRUPTOR. https://youtu.be/BqI6Ak1lZNs
        resume.SetActive(isPaused); // ACTIVA EL PANEL DE PAUSA.
        ToggleButtons(!isPaused); // DESACTIVA LOS BOTONES PARA MÓVILES.
    }

    public void RestartGame() // FUNCIÓN PARA REINICIAR EL JUEGO TRAS UN GAME OVER.
    {
        KillMinos(); // ELIMINA TODOS LOS TETRIMINOS DE LA PANTALLA QUE HAYAN QUEDADO.
        Awake(); // VUELVE A REINICIAR TODAS LAS VARIABLES A SUS ESTADOS ORIGINALES.
    }

    public void MuteMusic() /* FUNCIÓN INTERRUPTOR PARA MUTEAR LA MÚSICA DEL JUEGO. */
    {
        isMute = !isMute; /* CADA VEZ QUE SE INVOQUE isMute INTERCAMBIA SU ESTADO. */

        if (isMute) /* SI ESTÁ MUTEADO... */
        {
            un_Mute_img.sprite = mute_graph; /* LE PONGO EL SPRITE DEL ALTAVOZ MUTEADO */
            asMusic.Stop(); /* https://i.imgflip.com/1yzsx2.jpg */
        }
        else
        {
            un_Mute_img.sprite = unMute_graph; /* SI NO ESTÁ MUTEADO, LE PONGO EL DE NO MUTEADO */
            asMusic.Play(); /* https://images-cdn.9gag.com/photo/a6OWWz9_460s.jpg */
        }
    }

    public void QuitGame() // FUNCIÓN PARA QUITAR EL JUEGO PARA MÓVILES.
    {
        Application.Quit(); // SALTE DE LA APP.
    }
}