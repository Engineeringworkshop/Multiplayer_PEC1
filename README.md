#PEC 1

Pablo Molina Parellada

#1  Enlaces

Repositorio GitHub: https://github.com/Engineeringworkshop/Multiplayer_PEC1

Vídeo YouTube: https://youtu.be/21njbFpLa5I

#2  Tareas obligatorias

##2.1  Selección de número de jugadores:

He añadido un panel con texto y componente botón incorporado. De esta forma, se puede seleccionar el número inicial de jugadores de forma sencilla e intuitiva. Cada uno de los botones llama a la misma función del GameManager pero con distinto valor del argumento, indicando el número de jugadores iniciales.
Para mejorar la gestión de jugadores he añadido un array de colores, uno de spawnpoints y una lista que contendrá los jugadores activos. De esta forma, puedo añadir los tanques configurados a la lista, con lo que se podrán remover igual de fácil si los jugadores dejan la partida. Los colores y los spawn points se asignan por orden hasta alcanzar el número de jugadores.

He añadido el tank manager como componente del propio tanque, esto facilita la gestión y configuración. La configuración de las cámaras se hace ahora desde el CameraControlCinemachine, no desde el GameManager. He añadido también una referencia al GameManager en el TankManager para poder avisar al GameManager cuando un jugador ha sido derrotado y este pueda informar al CameraControlCinemachine de ello. Podría haberlo controlado directamente, pero de esta forma, según crezca la complejidad del juego, será más fácil gestionar las nuevas mecánicas (como la gestión de la conexión o desconexión de un jugador).

##2.2  División de pantalla:

La división de pantalla la he realizado con Cinemachine. He añadido cámaras virtuales y sus correspondientes cámaras para renderizar.
 
Las cámaras de 1 a 4 corresponden a las cámaras para los 4 jugadores. En caso de ser menos jugadores, las cámaras restantes se desactivarán. La cámara 5 corresponde a la cámara del minimapa. Una cámara aérea fija.

Para evitar que los últimos frames de las cámaras queden permanentemente y en lugar de usar el cullingMask = 0 y desactivar la cámara en el siguiente frame. He añadido una cámara de fondo completamente negra BlackCamera. De esta forma he simplificado la gestión de las cámaras.

La gestión del layout de cámaras se hace con 3 funciones específicas: TwoPlayers, ThreePlayers o FourPlayers. Cada una de ellas activa las cámaras necesarias y desactiva las demás.
La gestión de los jugadores se hace desde el script Player Manager. Enlista los jugadores según se van uniendo a la partida (son instanciados) para facilitar su control. Si un jugador es eliminado, este llamará a la función Tank Defeated del Game Manager que se encargará de reasignar las cámaras a los jugadores activos y mandar actualizar el layout de cámaras.

##2.3  Incorporación de nuevos jugadores:

Mediante el New Input System. Este asset tiene la componente Player Input Manager. Con ella he definido la tecla que quiero que escuche para añadir un jugador nuevo. En este caso, usando una ya definida en el Action Map.

El juego está programado para asignar los controles de los dos primeros jugadores a los controles de teclado (keyboard_1 y keyboard_2). Si no hiciéramos esto, no sería posible añadir dos jugadores que usaran el teclado a posteriori mediante la correspondiente tecla asignada.

Para evitar que se pueda volver a incorporar un jugador en la partida una vez derrotado, he modificado la forma en que un jugador es eliminado. En vez de desactivar el Game Object (lo cual desactiva también el Input Manager) he hecho una desactivación selectiva de componentes. Las componenets que he desactivado son las que interaccionan con el juego: script de movimiento, el de disparo, Rigidbody (configurado a kinematico), box collider, el padre de los renders y el canvas de la vida. Para poder comprobar si está activo o no, he añadido el bool isActive para este fin. He modificado las funciones que dependían de si el tanque estaba activado o no, al igual que he tenido que crear un método para resetear el disco de salud.

##2.4  Reiniciar el juego a la selección de jugadores:

Guardando los jugadores en una lista. De esta forma, al iniciar el bucle de la partida es fácil gestionar las coroutines ya programadas en el juego original.

#3  Tareas opcionales

##3.1  Minimapa:

Una cámara virtual ortográfica desde una posición cenital con suficiente tamaño para englobar todo el mapa.

##3.2  Cinemachine:

Añadiendo el paquete cinemachine. He añadido una cámara virtual para cada jugador y una para el minimapa con sus correspondientes cámaras con el componente cinemachine brain. Las cámaras se manejas desde un script “CameraControlCinemachine” que las activará, desactivará y cambiará de tamaño según la cantidad de jugadores. He añadido una cámara cenital para hacer de minimapa.

##3.3  New Input system:

He añadido el new input system al proyecto. He definido un único action map “Gameplay” con las acciones: para mover, disparar, disparo alternativo y unirse a la partida. Este action map lo he asignado al esquema de teclado y luego lo he duplicado a un segundo teclado y vuelto a duplicar para signarlo a controller (mando genérico). 

#4  Mejoras adicionales

##4.1  Menú de inicio: se tiene que permitir iniciar la partida fácilmente.

Menú en pantalla con 3 opciones: 2 Jugadores, 3 jugadores y 4 jugadores.

##4.2  Selector de jugadores: tiene que ser fácil e intuitivo.

Menú en pantalla con 3 opciones: 2 Jugadores, 3 jugadores y 4 jugadores.

##4.3  Pantalla dividida dinámica: con la entrada y muerte de los jugadores la pantalla se tiene que reajustar correctamente.

Con cinemachine. Se reajustan automáticamente cuando un jugador es eliminado de la ronda.

##4.4  Controles bien definidos: que no tengan solapamientos y que sean fáciles de remapear.

Con el new Input manager es fácil volver a definir los controles. Como hemos creado un solo action map podemos definir las nuevas teclas para cada dispositivo de forma separada.

##4.5  Comportamiento de cámara huérfana: al morir un tanque la cámara tiene que controlarse adecuadamente y se le da un buen uso.

La cámara se desactiva y las cámaras de los jugadores activos se redimensionan acorde.

##4.6  Fin de partida: contabilización correcta de las victorias de todas las rondas en una partida.

Ya implementado en la versión original

##4.7  Reinicio del juego: se restablece el juego, tanto en cada ronda como en cada partida, donde se reiniciará a la primera ronda.

Ya implementado en la versión original. Con una pequeña modificación en la función de start se cargan los jugadores correspondientes al inicio de cada ronda.

##4.8  Interfaz gráfica: elementos de la UI trabajados.

He mantenido el estilo original del juego. El menú de selección de número de jugadores utiliza el mismo estilo de texto que los mensajes del juego original.

##4.9  Inclusión de información de debug sólo en caso necesario.

La información que se muestra en el debug es:

- En caso de intentar asignar más jugadores que controles. Los dos primeros jugadores usarán el teclado y los dos siguientes un controller distinto cada uno. Si no hay controllers suficientes conectados se mostrará un mensaje por el debug.
