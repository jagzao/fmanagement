CREATE TABLE Users (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Pass VARCHAR(100) NOT NULL
);

CREATE TABLE Bills (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Company VARCHAR(255) NOT NULL,
    Date TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE Files (
    Id SERIAL PRIMARY KEY,
    FilePath TEXT,
    FileSize BIGINT,
    Name TEXT,
    ParentId TEXT,
    IsDirectory BOOLEAN
);

--  Stored Procedures --------------------------------
----------------------------------------------------------------
-- Procedimiento para insertar un archivo
CREATE OR REPLACE PROCEDURE public.insert_file(
    file_path TEXT, 
    file_size BIGINT, 
    name TEXT, 
    parent_id INT, 
    is_directory BOOLEAN)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO public.files (FilePath, FileSize, Name, ParentId, IsDirectory)
    VALUES (file_path, file_size, name, parent_id, is_directory);
END;
$$;

-- Procedimiento para actualizar un archivo
CREATE OR REPLACE PROCEDURE public.update_file(
    p_id INT,
    p_file_path TEXT, 
    p_file_size BIGINT, 
    p_name TEXT, 
    p_parent_id INT, 
    p_is_directory BOOLEAN)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE public.files
    SET FilePath = p_file_path,
        FileSize = p_file_size,
        Name = p_name,
        ParentId = p_parent_id,
        IsDirectory = p_is_directory
    WHERE Id = p_id;
END;
$$;

-- Procedimiento para eliminar un archivo y sus hijos, incluyendo sus archivos físicos

-- Obtener todos los children del parent
CREATE OR REPLACE FUNCTION get_all_file_children(file_id INT)
RETURNS TABLE(Id INT, FilePath TEXT) AS $$
BEGIN
    RETURN QUERY
    WITH RECURSIVE file_cte AS (
        SELECT f.Id, f.FilePath
        FROM public.files f
        WHERE f.Id = file_id
        UNION ALL
        SELECT f.Id, f.FilePath
        FROM public.files f
        INNER JOIN file_cte fc ON f.ParentId::INT = fc.Id
    )
    SELECT f.Id, f.FilePath FROM file_cte f;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE PROCEDURE delete_file_with_children(file_id INT)
LANGUAGE plpgsql
AS $$
BEGIN
    DELETE FROM public.files WHERE Id IN (SELECT Id FROM get_all_file_children(file_id));
END;
$$;


-- Función para obtener todos los archivos
CREATE OR REPLACE FUNCTION public.get_all_files()
RETURNS TABLE(
    file_id INT,
    file_name TEXT,
    file_parent_id TEXT,
    file_is_directory BOOLEAN,
    file_size BIGINT,
    file_path TEXT
) AS $$
BEGIN
    RETURN QUERY
    SELECT
        id AS file_id,
        name AS file_name,
        parentid AS file_parent_id,
        isdirectory AS file_is_directory,
        filesize AS file_size,
        filepath AS file_path
    FROM files;
END;
$$ LANGUAGE plpgsql;

-- Función para obtener un archivo por su ID
CREATE OR REPLACE FUNCTION get_file_by_id(file_id INT)
RETURNS TABLE(id INT, file_path TEXT, file_size BIGINT, name TEXT, parent_id INT, is_directory BOOLEAN)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY 
    SELECT id, file_path, file_size, name, parent_id, is_directory 
    FROM files 
    WHERE id = file_id;
END;
$$;

-- Función para generar un informe
CREATE OR REPLACE FUNCTION public.generate_report(start_date TIMESTAMP, end_date TIMESTAMP)
RETURNS TABLE(id INT, name VARCHAR(255), company VARCHAR(255), date TIMESTAMP)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT b.id, b.name, b.company, b.date
    FROM bills b
    WHERE b.date BETWEEN start_date AND end_date;
END;
$$;