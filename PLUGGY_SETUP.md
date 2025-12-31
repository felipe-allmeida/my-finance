# Configuração Pluggy - Guia Completo

## 1. Criar Conta na Pluggy

1. Acesse [https://pluggy.ai](https://pluggy.ai)
2. Crie uma conta (ou faça login)
3. Acesse o Dashboard
4. Copie suas credenciais:
   - **Client ID**
   - **Client Secret**

## 2. Configurar Credenciais

Edite o arquivo `src/API/MyFinance.Api/appsettings.json`:

```json
{
  "Pluggy": {
    "ClientId": "seu-client-id-aqui",
    "ClientSecret": "seu-client-secret-aqui"
  }
}
```

## 3. Executar a Aplicação

### Opção A: Docker (Recomendado)

```bash
docker-compose up -d
```

A API estará disponível em: `http://localhost:8080`

### Opção B: Desenvolvimento Local

```bash
# Terminal 1 - Banco de dados
docker-compose up myfinance.postgres myfinance.redis

# Terminal 2 - API
cd src/API/MyFinance.Api
dotnet run
```

## 4. Conectar ao Seu Banco

### Passo 1: Criar Connect Token

```bash
curl -X POST http://localhost:8080/api/pluggy/connect-token
```

Resposta:
```json
{
  "accessToken": "eyJhbGc..."
}
```

### Passo 2: Usar o Widget Pluggy (Frontend)

Você precisa criar uma página HTML com o widget Pluggy:

```html
<!DOCTYPE html>
<html>
<head>
  <title>Conectar Banco - MyFinance</title>
  <script src="https://cdn.pluggy.ai/connect/v2/pluggy-connect.js"></script>
</head>
<body>
  <button id="open-pluggy">Conectar Banco</button>

  <script>
    const connectToken = 'SEU_TOKEN_AQUI'; // Token do passo 1

    const pluggyConnect = PluggyConnect.init({
      connectToken: connectToken,
      includeSandbox: true, // Apenas para testes
      onSuccess: (itemData) => {
        console.log('Conexão bem-sucedida!', itemData);

        // Enviar itemId para sua API
        fetch('http://localhost:8080/api/pluggy/connections', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            itemId: itemData.item.id,
            userId: '00000000-0000-0000-0000-000000000000' // TODO: Usar ID real do usuário
          })
        })
        .then(response => response.json())
        .then(data => console.log('Conexão armazenada:', data))
        .catch(error => console.error('Erro:', error));
      },
      onError: (error) => {
        console.error('Erro na conexão:', error);
      }
    });

    document.getElementById('open-pluggy').onclick = () => {
      pluggyConnect.openFlow();
    };
  </script>
</body>
</html>
```

### Passo 3: Webhook Automático

Quando você conectar um banco através do widget, a Pluggy enviará um webhook para:
```
POST http://localhost:8080/api/pluggy/webhook
```

⚠️ **Importante**: Para desenvolvimento local, você precisa usar um serviço como [ngrok](https://ngrok.com/) para expor sua API:

```bash
ngrok http 8080
```

Configure o webhook URL no dashboard da Pluggy para: `https://seu-url.ngrok.io/api/pluggy/webhook`

## 5. Endpoints Disponíveis

### Criar Connect Token
```http
POST /api/pluggy/connect-token
```

### Armazenar Conexão
```http
POST /api/pluggy/connections
Content-Type: application/json

{
  "itemId": "item_abc123",
  "userId": "user-guid-here"
}
```

### Webhook (chamado pela Pluggy)
```http
POST /api/pluggy/webhook
Content-Type: application/json

{
  "event": "item/updated",
  "data": {
    "itemId": "item_abc123"
  }
}
```

### Listar Transações
```http
GET /api/ledger/transactions
```

## 6. Fluxo Completo

1. **Frontend**: Solicita Connect Token → `POST /api/pluggy/connect-token`
2. **Frontend**: Abre widget Pluggy com o token
3. **Usuário**: Seleciona banco e faz login no widget
4. **Frontend**: Recebe `itemId` do callback de sucesso
5. **Frontend**: Armazena conexão → `POST /api/pluggy/connections`
6. **Pluggy**: Envia webhook → `POST /api/pluggy/webhook`
7. **Backend**: Sincroniza transações automaticamente
8. **Frontend**: Consulta transações → `GET /api/ledger/transactions`

## 7. Swagger/OpenAPI

Acesse a documentação interativa em:
```
http://localhost:8080/swagger
```

## 8. Troubleshooting

### Erro: "Pluggy credentials not configured"
- Verifique se preencheu as credenciais no `appsettings.json`

### Webhook não está funcionando
- Use ngrok para expor sua API local
- Configure a URL do webhook no dashboard da Pluggy

### Transações não aparecem
- Verifique os logs da aplicação
- Verifique a tabela `pluggy.sync_histories` no banco de dados

## 9. Banco de Dados

### Ver Sync Histories
```sql
SELECT * FROM pluggy.sync_histories ORDER BY created_at DESC;
```

### Ver Conexões
```sql
SELECT * FROM pluggy.user_connections WHERE is_active = true;
```

### Ver Transações Importadas
```sql
SELECT * FROM ledger.transactions
WHERE external_source = 'Pluggy'
ORDER BY date DESC;
```

## 10. Próximos Passos

- [ ] Implementar autenticação de usuários
- [ ] Criar frontend para gerenciar conexões
- [ ] Implementar mapeamento de categorias
- [ ] Adicionar suporte a múltiplos bancos por usuário
- [ ] Implementar sync manual de transações

## Documentação Adicional

- [Pluggy API Docs](https://docs.pluggy.ai)
- [Pluggy Connect Widget](https://docs.pluggy.ai/docs/connect-widget)
- [Pluggy Webhooks](https://docs.pluggy.ai/docs/webhooks)
