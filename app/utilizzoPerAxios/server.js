import express from 'express';
import cors from 'cors';
import multer from 'multer';
import sql from 'mssql';
import fs from 'fs';
import dotenv from 'dotenv';

// Load environment variables from .env file
dotenv.config();

console.log('Loaded environment variables:');
console.log('SQL Server:', process.env.SQL_SERVER);
console.log('SQL Database:', process.env.SQL_DATABASE);

const app = express();
const port = 1433; // New port

app.use(cors());

const uploadDir = 'uploads/';
if (!fs.existsSync(uploadDir)) {
    fs.mkdirSync(uploadDir, { recursive: true });
}

const storage = multer.diskStorage({
    destination: function (req, file, cb) {
        cb(null, uploadDir);
    },
    filename: function (req, file, cb) {
        cb(null, file.originalname);
    }
});

const upload = multer({ storage: storage });

const sqlConfig = {
    server: process.env.SQL_SERVER,
    database: process.env.SQL_DATABASE,
    options: {
        trustedConnection: true, // Utilizza l'autenticazione di Windows
        encrypt: true, // Se necessario
        trustServerCertificate: true, // Se necessario
        connectTimeout: 30000 // Timeout di connessione
    }
};


app.post('/upload', upload.single('video'), async (req, res) => {
    try {
        console.log('File details:', req.file);
        await saveVideoDetails(req.file);
        res.send('File uploaded successfully');
    } catch (err) {
        console.error('Error saving video details:', err);
        res.status(500).send('Error saving video details');
    }
});

async function saveVideoDetails(file) {
    try {
        let pool = await sql.connect(sqlConfig);
        console.log('Connected to SQL Server');

        let insertQuery = "INSERT INTO dbo.video (path, stato) VALUES (@path, @stato)";
        await pool.request()
            .input('path', sql.NVarChar, file.path)
            .input('stato', sql.Int, 0)
            .query(insertQuery);

        console.log(`File details saved: ${file.originalname}`);
    } catch (err) {
        console.error('SQL Server error:', err);
        throw err;
    } finally {
        sql.close();
    }
}

async function startServer() {
    try {
        await sql.connect(sqlConfig);
        console.log('Connected to SQL Server');
        app.listen(port, () => {
            console.log(`Server running at http://localhost:${port}`);
        });
    } catch (err) {
        console.error('Error connecting to SQL Server:', err.message);
    }
}

async function testDatabaseConnection() {
    try {
        let pool = await sql.connect(sqlConfig);
        let result = await pool.request().query('SELECT 1 AS Result');
        console.log('Test query result:', result.recordset);
    } catch (err) {
        console.error('Error testing database connection:', err.message);
    }
}

startServer();
testDatabaseConnection();
