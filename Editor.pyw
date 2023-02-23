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
D_Formables = {}

currentID = "FRA"
currentPROVID = "1"



# ----------------



fenmap = Tk()
fenmap.title("GSG - Europe Editor")


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

def SaveJSON():
    global D_Points,D_Provinces,D_Pays,D_NumberToCorrectProvince,D_ProvinceToNumber,D_Drapeaux,D_Formables
    with io.open("EditorOutput/points.json","w",encoding="utf8") as jsonp_file:

        jsonp_file.write("{\n\t\"points\":[\n")
        i = 1
        for point in D_Points:
            jsonp_file.write("\t\t{\"name\":\""+point+"\",\"x\":"+str(D_Points[point][0])+",\"y\":0.2,\"z\":"+str(-D_Points[point][1])+"}")

            if i != len(D_Points):
                jsonp_file.write(",")
            jsonp_file.write("\n")

            i+=1
        jsonp_file.write("\t]\n}")

    with io.open("EditorOutput/provinces.json","w",encoding="utf8") as jsonp_file:

        jsonp_file.write("{\n\t\"provinces\":[\n")

        j = 1
        for province in D_Provinces:
            jsonp_file.write("\t\t{\n")
            jsonp_file.write("\t\t\t\"id\":\""+D_ProvinceToNumber[province]+"\",\n")
            jsonp_file.write("\t\t\t\"name\":\""+province+"\",\n")
            jsonp_file.write("\t\t\t\"vertices\":[\n")
            for i in range(len(D_Provinces[province]["points"])):
                jsonp_file.write("\t\t\t\t\""+D_Provinces[province]["points"][i]+"\"")
                if i != len(D_Provinces[province]["points"])-1:
                    jsonp_file.write(",")
                jsonp_file.write("\n")
            jsonp_file.write("\t\t\t]\n\t\t}")

            if j != len(D_Provinces):
                jsonp_file.write(",")
            jsonp_file.write("\n")

            j+=1
        jsonp_file.write("\t]\n}")

    with io.open("EditorOutput/pays.json","w",encoding="utf8") as jsonp_file:
        
        jsonp_file.write("{\n\t\"pays\":[\n")
        i = 1
        for pays in D_Pays:
            jsonp_file.write("\t\t{\n")
            jsonp_file.write("\t\t\t\"id\":\""+pays+"\",\n")
            jsonp_file.write("\t\t\t\"name\":\""+D_Pays[pays]["name"]+"\",\n")
            jsonp_file.write("\t\t\t\"culture\":\""+D_Pays[pays]["culture"]+"\",\n")
            jsonp_file.write("\t\t\t\"color\":"+str(D_Pays[pays]["color"])+",\n")
            jsonp_file.write("\t\t\t\"secretNation\":\""+D_Pays[pays]["secretNation"]+"\"\n")
            jsonp_file.write("\t\t}")

            if i != len(D_Pays):
                jsonp_file.write(",")
            jsonp_file.write("\n")

            i+=1
        jsonp_file.write("\t]\n}")

    with io.open("EditorOutput/history.json","w",encoding="utf8") as jsonp_file:
        
        jsonp_file.write("{\n\t\"pays\":[\n")
        j = 1
        for pays in D_Pays:
            jsonp_file.write("\t\t{\n")
            jsonp_file.write("\t\t\t\"id\":\""+pays+"\",\n")
            jsonp_file.write("\t\t\t\"governement\":"+str(D_Pays[pays]["governement"])+",\n")

            jsonp_file.write("\t\t\t\"parties\":[\n")
            for i in range(5):
                jsonp_file.write("\t\t\t\t{\n")
                jsonp_file.write("\t\t\t\t\t\"name\":\""+D_Pays[pays]["parties"][i]["name"]+"\",\n")
                jsonp_file.write("\t\t\t\t\t\"popularity\":"+str(D_Pays[pays]["parties"][i]["popularity"])+"\n")
                jsonp_file.write("\t\t\t\t}")
                if i != 4:
                    jsonp_file.write(",")
                jsonp_file.write("\n")
            jsonp_file.write("\t\t\t],\n")


            jsonp_file.write("\t\t\t\"provinces\":[\n")
            for i in range(len(D_Pays[pays]["provinces"])):
                jsonp_file.write("\t\t\t\t\""+str(D_ProvinceToNumber[D_Pays[pays]["provinces"][i]])+"\"")
                if i != len(D_Pays[pays]["provinces"])-1:
                    jsonp_file.write(",")
                jsonp_file.write("\n")
            jsonp_file.write("\t\t\t],\n")

            jsonp_file.write("\t\t\t\"cores\":[\n")
            for i in range(len(D_Pays[pays]["cores"])):
                jsonp_file.write("\t\t\t\t\""+str(D_ProvinceToNumber[D_Pays[pays]["cores"][i]])+"\"")
                if i != len(D_Pays[pays]["cores"])-1:
                    jsonp_file.write(",")
                jsonp_file.write("\n")
            jsonp_file.write("\t\t\t]\n")
            jsonp_file.write("\t\t}")

            if j != len(D_Pays):
                jsonp_file.write(",")
            jsonp_file.write("\n")
            j+=1
        jsonp_file.write("\t]\n}")


    with io.open("EditorOutput/formables.json","w",encoding="utf8") as jsonp_file:
        
        jsonp_file.write("{\n\t\"formables\":[\n")
        j = 1
        for pays in D_Formables:
            jsonp_file.write("\t\t{\n")
            jsonp_file.write("\t\t\t\"id\":\""+pays+"\",\n")
            jsonp_file.write("\t\t\t\"name\":\""+D_Formables[pays]["name"]+"\",\n")


            jsonp_file.write("\t\t\t\"contestants\":[\n")
            for i in range(len(D_Formables[pays]["contestants"])):
                jsonp_file.write("\t\t\t\t\""+D_Formables[pays]["contestants"][i]+"\"")
                if i != len(D_Formables[pays]["contestants"])-1:
                    jsonp_file.write(",")
                jsonp_file.write("\n")
            jsonp_file.write("\t\t\t],\n")

            jsonp_file.write("\t\t\t\"required\":[\n")
            for i in range(len(D_Formables[pays]["required"])):
                jsonp_file.write("\t\t\t\t\""+str(D_ProvinceToNumber[D_Formables[pays]["required"][i]])+"\"")
                if i != len(D_Formables[pays]["required"])-1:
                    jsonp_file.write(",")
                jsonp_file.write("\n")
            jsonp_file.write("\t\t\t]\n")
            jsonp_file.write("\t\t}")

            if j != len(D_Formables):
                jsonp_file.write(",")
            jsonp_file.write("\n")
            j+=1
        jsonp_file.write("\t]\n}")
            

def LoadJSONs():
    global D_Points,D_Provinces,D_Pays,D_NumberToCorrectProvince,D_ProvinceToNumber,D_Drapeaux,D_Formables
    with io.open("Assets/Resources/JSON/points.json","r",encoding="utf8") as jsonp_file:
        json = eval(jsonp_file.read())["points"]
        for point in json:
            D_Points[point["name"]] = [point["x"],-point["z"]]
        
    with io.open("Assets/Resources/JSON/provinces.json","r",encoding="utf8") as jsonp_file:
        json = eval(jsonp_file.read())["provinces"]
        
        for province in json:
            D_Provinces[province["id"]] = {}
            D_Provinces[province["id"]]["name"] = province["name"]
            D_Provinces[province["id"]]["owner"] = "000"
            D_Provinces[province["id"]]["points"] = province["vertices"]
            listPoints = [D_Points[point] for point in province["vertices"]] 
            canvasmap.create_polygon(listPoints, outline='black',fill="gray", width=1,tags=province["id"])
            canvasmap.tag_bind(province["id"],"<Button-3>",RightClick)

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
            D_Pays[pays["id"]]["cores"] = pays["cores"]

            for i in range(len(pays["provinces"])):
                D_Provinces[pays["provinces"][i]]["owner"] = pays["id"]
                canvasmap.itemconfigure(pays["provinces"][i],fill='#%02x%02x%02x' % tuple(D_Pays[pays["id"]]["color"]))

    for Pays in D_Pays:
        D_Drapeaux[Pays] = {}
        for Parti in ["republic","monarchy","communism","fascism"]:
            Path = "Assets/Resources/Flags/"+Pays+"_"+Parti+".png"
            if not os.path.exists(Path):
                Path = Path = "Assets/Resources/Flags/"+Pays+".png"
            D_Drapeaux[Pays][Parti] = ImageTk.PhotoImage(Image.open(Path).resize((200, 150)))


    with io.open("Assets/Resources/JSON/formables.json","r",encoding="utf8") as jsonp_file:
        json = eval(jsonp_file.read())["formables"]
        for formable in json:
            D_Formables[formable["id"]] = {}
            D_Formables[formable["id"]]["name"] = formable["name"]
            D_Formables[formable["id"]]["contestants"] = formable["contestants"]
            D_Formables[formable["id"]]["required"] = formable["required"]

    #SaveJSON()


def RightClick(event): # Paint
    global D_Pays,D_Provinces
    idNum = event.widget.gettags('current')[0]

    D_Pays[D_Provinces[idNum]["owner"]]["provinces"].remove(idNum)
    D_Pays[currentID]["provinces"].append(idNum)
    D_Provinces[idNum]["owner"] = currentID
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
