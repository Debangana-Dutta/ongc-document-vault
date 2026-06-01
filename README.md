SELECT column_name FROM information_schema.columns WHERE table_name = 'user_dataset_access';
SELECT column_name FROM information_schema.columns WHERE table_name = 'user_metadata_policy';

ALTER TABLE users ALTER COLUMN password DROP NOT NULL;
-- Use the exact column names confirmed by your SELECT query
CREATE INDEX IF NOT EXISTS idx_uda_cpf ON user_dataset_access (userid);
CREATE INDEX IF NOT EXISTS idx_ump_cpf ON user_metadata_policy (user_cpf);
CREATE INDEX IF NOT EXISTS idx_docs_source ON indexed_documents (source_excel_file);

SELECT d.datasetname AS dataset_name
FROM user_dataset_access uda
JOIN datasets d ON d.datasetid = uda.datasetid
WHERE uda.userid = (SELECT id FROM users WHERE cpf = '101');

SELECT * FROM datasets;
SELECT d.datasetname AS dataset_name
FROM user_dataset_access uda
JOIN datasets d ON d.datasetid = uda.datasetid
WHERE uda.userid = 2;
SELECT * FROM user_dataset_access;

SELECT d.datasetname AS dataset_name
FROM user_dataset_access uda
JOIN datasets d ON d.datasetid = uda.datasetid
WHERE uda.userid = 1;
INSERT INTO user_dataset_access (userid, datasetid) VALUES (2, 1);
INSERT INTO datasets (datasetid, datasetname) VALUES (1, 'Sample_Dataset_Name');
SELECT * FROM datasets;
SELECT d.datasetname AS dataset_name
FROM user_dataset_access uda
JOIN datasets d ON d.datasetid = uda.datasetid
WHERE uda.userid = (SELECT id FROM users WHERE cpf = '101');

INSERT INTO user_dataset_access (userid, datasetid) VALUES (2, 1);

// new 
1.ALTER TABLE users ADD CONSTRAINT uq_users_cpf UNIQUE (cpf);
2.  **Create Dataset Lookup Table**
    This table resolves the data type mismatch error by providing an integer Primary Key (`datasetid`) for every dataset name.
    ```sql
    CREATE TABLE IF NOT EXISTS datasets (
      datasetid SERIAL PRIMARY KEY,
      datasetname TEXT NOT NULL UNIQUE
    );
3. INSERT INTO datasets (datasetname)
  SELECT DISTINCT source_excel_file
  FROM   indexed_documents
  WHERE  source_excel_file IS NOT NULL
ON CONFLICT (datasetname) DO NOTHING;
