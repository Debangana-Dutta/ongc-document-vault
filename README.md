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
