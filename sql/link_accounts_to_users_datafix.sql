UPDATE accounts
SET user_id = source.user_id
FROM UNNEST (@AccountIds, @UserIds) AS SOURCE(account_id, user_id)
WHERE accounts.id = source.account_id;