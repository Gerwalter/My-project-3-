def clean_merge_conflicts(input_file, output_file):
    """
    Limpia los conflictos de merge en un archivo y guarda la versión corregida.
    NOTA: Este script se queda con la primera opción (HEAD) y descarta la otra.
    """
    with open(input_file, "r", encoding="utf-8") as f:
        lines = f.readlines()

    cleaned_lines = []
    skip = False
    for line in lines:
        if line.startswith("<<<<<<<"):
            skip = True  # Comienza el conflicto
            continue
        elif line.startswith("======="):
            # Se queda con la primera parte (HEAD), ignora lo siguiente
            skip = True
            continue
        elif line.startswith(">>>>>>>"):
            skip = False  # Termina el conflicto
            continue

        if not skip:
            cleaned_lines.append(line)

    with open(output_file, "w", encoding="utf-8") as f:
        f.writelines(cleaned_lines)


# Ejemplo de uso:
# Esto limpiará "Test 1.unity" y dejará la versión sin conflictos en "Test 1_clean.unity"
clean_merge_conflicts("Test 1.unity", "Test 1_clean.unity")

