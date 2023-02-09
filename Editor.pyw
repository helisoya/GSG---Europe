from tkinter import *
from random import *
from tkinter import simpledialog
from tkinter import messagebox
import os
import io
from PIL import Image, ImageTk

D_Points = {}
D_Provinces = {}
D_Pays = {}
D_Drapeaux = {}

D_NumberToCorrectProvince = {}
D_ProvinceToNumber = {}


currentID = "FRA"

# ----------------



fenmap = Tk()
fenmap.title("GSG 3 Editor")


Frame_Info = Frame(fenmap,highlightthickness = 1,highlightbackground="black",height=700,width=200)
Frame_Info.pack(side="left")
Frame_Info.pack_propagate(0) 
canvasmap = Canvas(fenmap,height=700,width=1000,bg="#007F7F")
canvasmap.pack(side="right")


# -------------------- Fonctions pour le déplacement de la carte -----------------


def move_start(event):
    canvasmap.scan_mark(event.x, event.y)
def move_move(event):
    canvasmap.scan_dragto(event.x, event.y, gain=1)

canvasmap.bind("<ButtonPress-1>",move_start)
canvasmap.bind("<B1-Motion>", move_move)


# -------------------- Fonctions Chargement Dictionaires -------------------------


def LoadJSONs():
    global D_Points,D_Provinces,D_Pays,D_NumberToCorrectProvince,D_ProvinceToNumber,D_Drapeaux
    with io.open("Assets/Resources/JSON/points.json","r",encoding="utf8") as jsonp_file:
        json = eval(jsonp_file.read())["points"]
        for point in json:
            D_Points[point["name"]] = [point["x"],-point["z"]]
        
    with io.open("Assets/Resources/JSON/provinces.json","r",encoding="utf8") as jsonp_file:
        json = eval(jsonp_file.read())["provinces"]
        
        i = 1
        for province in json:
            D_NumberToCorrectProvince[str(i)] = province["name"]
            D_ProvinceToNumber[province["name"]] = str(i)
            D_Provinces[province["name"]] = {}
            D_Provinces[province["name"]]["owner"] = "000"
            D_Provinces[province["name"]]["points"] = province["vertices"]
            listPoints = [D_Points[point] for point in province["vertices"]] 
            canvasmap.create_polygon(listPoints, outline='black',fill="gray", width=1,tags=str(i))
            canvasmap.tag_bind(str(i),"<Button-3>",RightClick)
            i+=1

    with io.open("Assets/Resources/JSON/pays.json","r",encoding="utf8") as jsonp_file:
        json = eval(jsonp_file.read())["pays"]
        for pays in json:
            D_Pays[pays["id"]] = {}
            D_Pays[pays["id"]]["name"] = pays["name"]
            D_Pays[pays["id"]]["color"] = pays["color"]
            D_Pays[pays["id"]]["culture"] = pays["culture"]
            D_Pays[pays["id"]]["secretNation"] = pays["secretNation"]
          
    with io.open("Assets/Resources/JSON/history.json","r",encoding="utf8") as jsonp_file:
        json = eval(jsonp_file.read())["pays"]
        for pays in json:
            D_Pays[pays["id"]]["governement"] = pays["governement"]
            D_Pays[pays["id"]]["parties"] = pays["parties"]
            D_Pays[pays["id"]]["provinces"] = pays["provinces"]

            for i in range(len(pays["provinces"])):
                D_Provinces[pays["provinces"][i]]["owner"] = pays["id"]
                canvasmap.itemconfigure(D_ProvinceToNumber[pays["provinces"][i]],fill='#%02x%02x%02x' % tuple(D_Pays[pays["id"]]["color"]))

    for Pays in D_Pays:
        D_Drapeaux[Pays] = {}
        for Parti in ["republic","monarchy","communism","fascism"]:
            Path = "Assets/Resources/Flags/"+Pays+"_"+Parti+".png"
            if not os.path.exists(Path):
                Path = Path = "Assets/Resources/Flags/"+Pays+".png"
            D_Drapeaux[Pays][Parti] = ImageTk.PhotoImage(Image.open(Path).resize((200, 150)))



def RightClick(event): # Paint
    global D_Pays,D_Provinces
    idNum = event.widget.gettags('current')[0]
    idNom = D_NumberToCorrectProvince[idNum]

    D_Pays[D_Provinces[idNom]["owner"]]["provinces"].remove(idNom)
    D_Pays[currentID]["provinces"].append(idNom)
    D_Provinces[idNom]["owner"] = currentID
    canvasmap.itemconfigure(idNum,fill='#%02x%02x%02x' % tuple(D_Pays[currentID]["color"]))

def WipeInfo():
    for element in Frame_Info.winfo_children():
        element.destroy()



# ----------------------- Fonctions relatives à la carte ------------------


def CenterPolygon(Coords):
    x = 0
    y = 0
    for i in range(len(Coords)):
        if i%2==0:
            x = x + Coords[i]
        else:
            y = y + Coords[i]
    x = x/(len(Coords)/2)
    y = y/(len(Coords)/2)
    return (x,y)


def convertIntToGovernement(i):
    if i <= 2:
        return "republic"
    if i <= 5:
        return "monarchy"
    if i <= 8:
        return "communism"
    return "fascism"

#--------------------------------------




LoadJSONs()

# ------------------------ GUI -------------------------------

def ChangeCurrentID(newVal):
    global currentID,Pays_Flag,governement

    for key in D_Pays:
        if newVal == D_Pays[key]["name"]:
            currentID = key
            governement.set(D_Pays[currentID]["governement"])
            Pays_Flag.configure(image=D_Drapeaux[currentID][convertIntToGovernement(D_Pays[currentID]["governement"])])
            Pays_Flag.image = D_Drapeaux[currentID][convertIntToGovernement(D_Pays[currentID]["governement"])]
            return

def ChangeGovernement():
    global Pays_Flag, D_Pays

    D_Pays[currentID]["governement"] = governement.get()
    Pays_Flag.configure(image=D_Drapeaux[currentID][convertIntToGovernement(D_Pays[currentID]["governement"])])
    Pays_Flag.image = D_Drapeaux[currentID][convertIntToGovernement(D_Pays[currentID]["governement"])]

idPays = StringVar()
governement = IntVar()

liste = []
for key in D_Pays:
    liste.append(D_Pays[key]["name"])

liste.sort()

for key in D_Pays:
    if D_Pays[key]["name"] == liste[0]:
        currentID = key
        break
idPays.set(liste[0])
options = OptionMenu(Frame_Info, idPays, *liste,command=ChangeCurrentID)
options.pack()

Pays_Flag = Label(Frame_Info,image=D_Drapeaux[currentID][convertIntToGovernement(D_Pays[currentID]["governement"])]) #Info Pays
Pays_Flag.pack()

governement.set(D_Pays[currentID]["governement"])
entryGov = Entry(Frame_Info,textvariable=governement)
entryGov.pack()

boutonGov = Button(Frame_Info,text="Change Governement",command=ChangeGovernement)
boutonGov.pack()

fenmap.mainloop()
