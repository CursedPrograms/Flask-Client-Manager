from flask import Flask, render_template, request, jsonify
import sqlite3
import json

app = Flask(__name__)

def init_db():
    connection = sqlite3.connect('clients.db')
    cursor = connection.cursor()
    cursor.execute('''
        CREATE TABLE IF NOT EXISTS clients (
            id INTEGER PRIMARY KEY,
            name TEXT NOT NULL,
            surname TEXT NOT NULL,
            email TEXT NOT NULL,
            client_code TEXT NOT NULL,
            num_of_contacts INTEGER NOT NULL
        )
    ''')
    connection.commit()
    connection.close()

def generate_client_code(name):
    name = name.upper().replace(" ", "")
    if len(name) < 3:
        name += "A"
    client_code = name[:3] + "{:03d}".format(0)
    
    connection = sqlite3.connect('clients.db')
    cursor = connection.cursor()
    while True:
        cursor.execute('SELECT COUNT(*) FROM clients WHERE client_code = ?', (client_code,))
        count = cursor.fetchone()[0]
        if count == 0:
            break
        client_code = client_code[:-3] + "{:03d}".format(int(client_code[-3:]) + 1)
    connection.close()
    
    return client_code

@app.route('/', methods=['GET', 'POST'])
def index():
    if request.method == 'POST':
        name = request.form.get('name')
        surname = request.form.get('surname') 
        email = request.form.get('email') 
        connection = sqlite3.connect('clients.db')
        cursor = connection.cursor()

        client_code = generate_client_code(name)

        cursor.execute('INSERT INTO clients (name, surname, email, client_code, num_of_contacts) VALUES (?, ?, ?, ?, ?)',
                       (name, surname, email, client_code, 0))       
        connection.commit()

        cursor.execute('SELECT COUNT(*) FROM clients')
        total_clients = cursor.fetchone()[0]
        cursor.execute('UPDATE clients SET num_of_contacts = ?', (total_clients,))
        connection.commit()

        connection.close()

    connection = sqlite3.connect('clients.db')
    cursor = connection.cursor()
    cursor.execute('SELECT * FROM clients ORDER BY name ASC')
    clients = cursor.fetchall()
    connection.close()

    return render_template('index.html', clients=clients)

@app.route('/get_clients', methods=['GET'])
def get_clients():
    connection = sqlite3.connect('clients.db')
    cursor = connection.cursor()
    cursor.execute('SELECT * FROM clients ORDER BY name ASC')
    clients = cursor.fetchall()
    connection.close()

    clients_data = []
    for client in clients:
        client_dict = {
            'id': client[0],
            'name': client[1],
            'surname': client[2], 
            'email': client[3],        
            'client_code': client[4],
            'num_of_contacts': client[5]           
        }
        clients_data.append(client_dict)

    with open('clients.json', 'w') as json_file:
        json.dump(clients_data, json_file)

    return jsonify(clients_data)

if __name__ == '__main__':
    init_db()     
    app.run(debug=True)