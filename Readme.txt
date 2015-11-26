#Code Disclaimer
* Esto es lo que es.
* S3 viene de Spell Stone Simulator
* El juego de cartas que simula se puede jugar en Kongregate.com
* La funcionalidad que tiene es la del juego.
* NO hay pruebas unitarias, me era mas facil probar con las batallitas directamente. Por eso esta la opcion de hacerla por turnos
* Muchas cosas son mejorables, pero es lo que tiene hacer las cosas sobre la marcha.
* En especial:
	* SystemExtensions no sirve para nada. Iba a hacer una extension de Int32 pero al final no la hice.
	* La carpeta (y espacio de nombres) Flows tenian sentido cuando empecé con WF. La excepcion se quedó ahi.
	* Creo que hay alguna clase de mas, pero tampoco me importa mucho.
	* No hacia falta clonar nada hasta que intenté el Parallel, pero ahora lo he dejado, que no  mete apenas overhead (lo medí con tandas de 1000 batallas y era menos de 0.5'') y me da posibilidades a futuro.
	* Aunque, por lo general, las clases estan preparadas para aceptar la ruta de los ficheros de datos, la aplicacion de consola no (ver modo de uso).
	* Por comodidad por mi parte, no hay apenas control de errores. Asi me salta y se que algo no va bien. Para paquetizarlo, hay que meter una gestion de errores decente.
	* La Consola Web no está ni empezada, solo creé el proyecto.

#Modo de Uso
* Hay que copiar los ficheros de cartas.csv y players.csv de la carpeta de resources a donde se vaya a ejecutar.
* Si no quereis que casque cual cabron, no los cambieis mucho. Espero haber copiado los buenos.
* Comandos: (no es case sensitive en los comandos)
	* 'quit': lo cierra
	* 'clear': lo pone todo a situacion inicial. Para lanzar otro comando.
	* 'Combat' o 'c':combate por turnos, muestra el resultado de cada turno de cada jugador. Es un poco confuso que no sea turno X fase A y B, pero el juego original cuenta un turno diferente en cada ataque de cada jugador.
		* opciones (precedidas de - y el valor despues de : (no poner espacios))
			-a:cadena => atacante de nombre 'cadena' (este si es CaseSensitive, soy asi). Busca entre los jugadores el que tenga ese nombre.
			-d:cadena => defensor de nombre 'cadena'
			-o:cadena => opciones del combate separadas por comas. De momento f y t (f,t). Sin orden especifico.
	* 'fight': combate de uno contra otro o todos (si no hay defensor).
		* Opciones: además de las anteriores
			-v:entero => repite cada enfrentamiento entero veces
	* 'war': sin atacante ni defensor. todos contra todos
		* opciones:
			-v
			-out:algo => saca los resultados a fichero. warTotal.csv y WarDetail.csv. No los tengais bloqueados con el excel.
	 'sim': intenta buscar una baraja mejor. En este caso tambien hay que copiar el fichero Inventory.csv. Modifica la baraja del atacante con las cartas del inventario.
		* -dp:entero => numero de barajas en cada vuelta. Es aproximado, se pueden tener hasta 3 más.
		* -w:entero => las veces que una baraja debe ser la mejor antes de dejar de buscar.
		* -var:entero => cartas a sustituir. Una baraja tiene 15 cartas. ilimitadas en el inventario.
		* -p:cadena => fichero de players, no lo usaria con espacios.
		* -i:cadena => fichero de inventario, ni lo usaria.
		* -a:cadena => atacante, es necesario.
		* -d:cadena => defensor, si no lo poneis, simula comtra todos los players.
		* -o:cadena => opciones del combate.

			

