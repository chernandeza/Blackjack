# Blackjack

El código de la aplicación se brinda con una cláusula "AS IS". Esto quiere decir que, la aplicación puede tener errores, 
problemas de lógica o "pulgas" que no han sido detectadas y que bajo ninguna circunstancia es obligación de 
mi persona solucionar.

Antes de ejecutar la aplicación de primera vez, se deben crear los eventos de bitácora utilizando una consola con derechos
de administración ("Run as Administrator") y deben ejecutarse los siguientes comandos.

eventcreate /ID 1 /L APPLICATION /T INFORMATION  /SO BlackJackGame /D "My first log"
eventcreate /ID 1 /L APPLICATION /T INFORMATION  /SO BlackJackClient /D "My first log"


Con estos comandos, se crearán eventos en la bitácora de aplicación de Windows donde se podrán ver errores, avance de la aplicación,
entre otros.
