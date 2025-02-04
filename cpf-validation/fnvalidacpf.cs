using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace cpf_validation
{
    public static class FnValidaCpf
    {
        [FunctionName("FnValidaCpf")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Iniciando a validação de CPF.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            if(data == null || data.cpf == null)
            {
                return new BadRequestObjectResult("Por favor, informe o CPF.");
            }

            string cpf = data.cpf.ToString();
            bool isValid = ValidateCPF(cpf);

            string responseMessage = isValid
                ? "CPF válido."
                : "CPF inválido.";
            return new OkObjectResult(responseMessage);
        }
    public static bool ValidateCPF(string cpf)
    {
        if (string.IsNullOrEmpty(cpf))
            return false;

        cpf = cpf.Replace(".", "").Replace("-", "");

        if (cpf.Length != 11)
            return false;

        bool igual = true;
        for (int i = 1; i < 11 && igual; i++)
            if (cpf[i] != cpf[0])
                igual = false;

        if (igual || cpf == "12345678909")
            return false;

        int[] numeros = new int[11];

        for (int i = 0; i < 11; i++)
            numeros[i] = int.Parse(cpf[i].ToString());

        int soma = 0;
        for (int i = 0; i < 9; i++)
            soma += (10 - i) * numeros[i];

        int resultado = soma % 11;

        if (resultado == 1 || resultado == 0)
        {
            if (numeros[9] != 0)
                return false;
        }
        else if (numeros[9] != 11 - resultado)
            return false;

        soma = 0;
        for (int i = 0; i < 10; i++)
            soma += (11 - i) * numeros[i];

        resultado = soma % 11;

        if (resultado == 1 || resultado == 0)
        {
            if (numeros[10] != 0)
                return false;
        }
        else
            if (numeros[10] != 11 - resultado)
            return false;

        return true;
    }
}
}
